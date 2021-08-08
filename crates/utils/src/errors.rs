use actix_web::ResponseError;
use async_graphql::{Error, ErrorExtensions};
use thiserror::Error;

#[derive(Error, Debug)]
pub enum TumblepubError {
  #[error("Not Found")]
  NotFound,
  #[error("Bad Request: {0}")]
  BadRequest(&'static str),
  #[error("Unauthorized")]
  Unauthorized,
  #[error(transparent)]
  InternalServerError(#[from] anyhow::Error),
}

impl From<reqwest::Error> for TumblepubError {
  fn from(err: reqwest::Error) -> Self {
    TumblepubError::InternalServerError(err.into())
  }
}

pub type Result<T> = ::std::result::Result<T, TumblepubError>;

// handle TumblepubErrors for GraphQL requests
impl ErrorExtensions for TumblepubError {
  fn extend(&self) -> Error {
    self.extend_with(|err, e| match err {
      TumblepubError::NotFound => e.set("code", "NOT_FOUND"),
      TumblepubError::BadRequest(_) => e.set("code", "BAD_REQUEST"),
      TumblepubError::Unauthorized => e.set("code", "UNAUTHORIZED"),
      TumblepubError::InternalServerError(_) => e.set("code", "INTERNAL_SERVER_ERROR"),
    })
  }
}

// handle TumblepubErrors for misc HTTP requests
impl ResponseError for TumblepubError {}
