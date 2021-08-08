use actix_web::{http::HeaderMap, HttpResponse, Responder};
use models::actor::Actor;
use openssl::{hash::MessageDigest, pkey::PKey, rsa::Rsa, sign::Verifier};
use serde::Serialize;

use tumblepub_utils::{
  crypto::Signature,
  errors::{Result, TumblepubError},
  HTTP_CLIENT,
};

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
