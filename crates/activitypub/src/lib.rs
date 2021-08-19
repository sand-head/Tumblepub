use actix_web::{http::HeaderMap, HttpResponse, Responder};
use base64::encode;
use chrono::Utc;
use itertools::Itertools;
use models::{activity::Activity, actor::Actor};
use once_cell::sync::Lazy;
use openssl::{
  hash::MessageDigest,
  pkey::PKey,
  rsa::Rsa,
  sign::{Signer, Verifier},
};
use regex::Regex;
use serde::Serialize;

use tumblepub_utils::{
  crypto::Signature,
  errors::{Result, TumblepubError},
  options::Options,
  Response, HTTP_CLIENT,
};
use url::Url;

static ACTOR_URI: Lazy<Regex> = Lazy::new(|| Regex::new("^https?://(.*)/@([a-z0-9_]+)$").unwrap());

pub mod conversion;
pub mod models;
pub mod webfinger;

pub trait ActivityPub {
  type ApType;
  fn as_activitypub(&self) -> Result<Self::ApType>;
}

pub fn activitypub_response<T>(body: &T) -> impl Responder
where
  T: Serialize,
{
  HttpResponse::Ok()
    .content_type("application/ld+json")
    .json(body)
}

async fn resolve_inbox_uri(actor_uri: &str) -> Result<String> {
  if actor_uri == "https://mastodon.social/inbox" {
    // todo: definitely handle this better
    return Ok(actor_uri.to_string());
  }
  // todo: we should probably cache inbox urls, either in memory or in db
  let actor = get_foreign_actor(actor_uri).await?;
  Ok(actor.inbox)
}

/// Delivers an [activity](Activity) to a single recipient's inbox.
async fn deliver_one(activity: &Activity, url: &str, private_key: &[u8]) -> Result<Response> {
  let builder = HTTP_CLIENT
    .post(url)
    .header("Content-Type", "application/activity+json")
    .json(activity);

  // todo: sign the request
  let inbox_uri = Url::parse(url).unwrap();
  let now = Utc::now().to_rfc2822();
  let signature = format!(
    "(request-target): post {}\nhost: {}\ndate: {}",
    inbox_uri.path(),
    inbox_uri.host_str().unwrap(),
    now.clone()
  );

  let key = PKey::from_rsa(Rsa::private_key_from_pem(private_key).unwrap()).unwrap();
  let mut signer = Signer::new(MessageDigest::sha256(), &key).unwrap();
  signer.update(signature.as_bytes()).unwrap();

  let builder = builder.header("Date", now).header(
    "Signature",
    format!(
      r#"keyId="{}#main-key",headers="(request-target) host date",signature="{}""#,
      activity.actor,
      encode(signer.sign_to_vec().unwrap())
    ),
  );

  let request = builder.build()?;
  Ok(HTTP_CLIENT.execute(request).await?)
}

/// Delivers an [activity](Activity) to all of its recipients.
pub async fn deliver(activity: Activity, private_key: &[u8]) -> Result<()> {
  // get all recipients
  // ensure that all recipients are unique
  // also, filter out the public URI
  let recipients = activity
    .to
    .iter()
    .merge(activity.cc.iter())
    .unique()
    .filter_map(|r| {
      (r != "https://www.w3.org/ns/activitystreams#Public").then(|| resolve_inbox_uri(r))
    })
    .collect_vec();

  // send activity to inbox of all recipients
  for recipient in recipients {
    let response = deliver_one(&activity, &recipient.await?, private_key).await?;
    (!response.status().is_client_error())
      .then(|| ())
      .ok_or(TumblepubError::BadRequest(
        "Recipient responded with a 4XX error.",
      ))?;
  }

  Ok(())
}

/// Obtains the blog name from a local URI.
pub fn get_name_from_uri(acct: &str) -> Result<String> {
  let captures = match ACTOR_URI.captures(acct) {
    Some(captures) => Ok(captures),
    None => Err(TumblepubError::NotFound),
  }?;

  let domain = captures
    .get(1)
    .ok_or(TumblepubError::BadRequest("no domain found"))?
    .as_str()
    .to_string();

  if domain != Options::get().local_domain {
    Err(TumblepubError::BadRequest(
      "blog is not local to this domain",
    ))
  } else {
    let blog_name = captures
      .get(2)
      .ok_or(TumblepubError::BadRequest("no blog name found"))?
      .as_str()
      .to_string();

    Ok(blog_name)
  }
}

// todo: move this to utils::crypto
pub async fn verify_signature(
  signature: Signature,
  headers: &HeaderMap,
  request_target: String,
) -> Result<bool> {
  // get public key pem from key ID (aka actor)
  let actor = get_foreign_actor(&signature.key_id).await?;
  let key =
    PKey::from_rsa(Rsa::public_key_from_pem(actor.public_key.public_key_pem.as_bytes()).unwrap())
      .unwrap();

  // create comparison vec and fill it with stuff from headers
  let mut comparison: Vec<String> = Vec::new();
  for header in signature.headers {
    comparison.push(match header.as_str() {
      "(request-target)" => format!("(request-target): post {}", request_target),
      _ => format!(
        "{}: {}",
        &header,
        headers
          .get(header.to_uppercase())
          .ok_or(TumblepubError::BadRequest(
            "could not get header for signature verification"
          ))?
          .to_str()
          .unwrap()
      ),
    });
  }

  // create verifier and verify the signature
  let mut verifier = Verifier::new(MessageDigest::sha256(), &key).unwrap();
  verifier.update(comparison.join("\n").as_bytes()).unwrap();
  Ok(verifier.verify(&signature.signature).unwrap())
}

pub async fn get_foreign_actor(link: &str) -> Result<Actor> {
  let request = HTTP_CLIENT
    .get(link)
    .header(
      "Accept",
      r#"application/ld+json; profile="https://www.w3.org/ns/activitystreams""#,
    )
    .build()?;
  let actor: Actor = HTTP_CLIENT.execute(request).await?.json().await?;
  Ok(actor)
}
