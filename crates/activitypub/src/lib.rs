use actix_web::{http::HeaderMap, HttpResponse, Responder};
use itertools::Itertools;
use models::{activity::Activity, actor::Actor};
use once_cell::sync::Lazy;
use openssl::{hash::MessageDigest, pkey::PKey, rsa::Rsa, sign::Verifier};
use regex::Regex;
use serde::Serialize;

use tumblepub_utils::{
  crypto::Signature,
  errors::{Result, TumblepubError},
  options::Options,
  Response, HTTP_CLIENT,
};

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

async fn deliver_one(activity: &Activity, url: &str) -> Result<Response> {
  let request = HTTP_CLIENT
    .post(url)
    .header("Content-Type", "application/activity+json")
    .json(activity)
    .build()?;

  // todo: sign the request

  Ok(HTTP_CLIENT.execute(request).await?)
}

/// Delivers an [activity](Activity) to all of its recipients.
pub async fn deliver(activity: Activity) -> Result<()> {
  // get all recipients
  // ensure that all recipients are unique
  // also, filter out the public URI
  // todo: resolve inboxes for all recipients
  let recipients = activity
    .to
    .iter()
    .merge(activity.cc.iter())
    .unique()
    .filter(|r| *r != "https://www.w3.org/ns/activitystreams#Public")
    .collect_vec();

  // send activity to inbox of all recipients
  for recipient in recipients {
    deliver_one(&activity, recipient).await?;
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
