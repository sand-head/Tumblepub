use std::future::{ready, Ready};

use actix_web::{Error, HttpRequest, HttpResponse, Responder};
use anyhow::Result;
use serde::Serialize;
use serde_json::Value;

pub mod crypto;

pub struct ActivityStreams<T = Value>(pub T);
impl<T: Serialize> Responder for ActivityStreams<T> {
  type Error = Error;
  type Future = Ready<Result<HttpResponse, Error>>;

  fn respond_to(self, _req: &HttpRequest) -> Self::Future {
    let body = serde_json::to_string(&self.0).unwrap();

    ready(Ok(
      HttpResponse::Ok()
        .content_type("application/ld+json")
        .body(body),
    ))
  }
}
