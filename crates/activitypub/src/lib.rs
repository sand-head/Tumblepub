use activitystreams::actor::{ApActor, Person};
use activitystreams_ext::Ext1;
use actix_web::{HttpResponse, Responder};
use extensions::public_key::PublicKey;
use serde::Serialize;

use tumblepub_utils::errors::Result as TpResult;

pub mod conversion;
pub mod extensions;
pub mod models;
pub mod webfinger;

pub type ApBlog = Ext1<ApActor<Person>, PublicKey>;

pub trait ActivityPub {
  type ApType;
  fn as_activitypub(&self) -> TpResult<Self::ApType>;
}

pub fn activitypub_response<T>(body: &T) -> impl Responder
where
  T: Serialize,
{
  HttpResponse::Ok()
    .content_type("application/ld+json")
    .json(body)
}
