use actix_web::{HttpResponse, ResponseError};
use juniper::{FieldError, IntoFieldError, Value};
use serde::Serialize;
use sqlx::Error as DBError;
use thiserror::Error;

#[derive(Debug, Error, Serialize)]
pub enum ServiceError {
  #[error("Internal Server Error")]
  InternalServerError,
  #[error("Bad Request: {0}")]
  BadRequest(String),
  #[error("Unauthorized")]
  Unauthorized,
  #[error("Could not connect to database")]
  DbConnectionError,
}

// match ServiceErrors to corresponding GraphQL responses
impl<S> IntoFieldError<S> for ServiceError {
  fn into_field_error(self) -> juniper::FieldError<S> {
    match self {
      ServiceError::InternalServerError => {
        FieldError::new("Internal Server Error", Value::<S>::Null)
      }
      ServiceError::BadRequest(msg) => FieldError::new(msg, Value::<S>::Null),
      ServiceError::Unauthorized => FieldError::new("Unauthorized", Value::<S>::Null),
      ServiceError::DbConnectionError => {
        FieldError::new("Could not connect to database", Value::<S>::Null)
      }
    }
  }
}

impl ResponseError for ServiceError {
  fn error_response(&self) -> actix_web::HttpResponse {
    match self {
      ServiceError::InternalServerError => {
        HttpResponse::InternalServerError().body("Internal Server Error")
      }
      ServiceError::BadRequest(ref msg) => HttpResponse::BadRequest().body(msg),
      ServiceError::Unauthorized => HttpResponse::Unauthorized().finish(),
      ServiceError::DbConnectionError => {
        if cfg!(debug_assertions) {
          HttpResponse::InternalServerError().body("Unable to connect to database")
        } else {
          HttpResponse::InternalServerError().body("Internal Server Error")
        }
      }
    }
  }
}

impl From<DBError> for ServiceError {
  fn from(error: DBError) -> Self {
    match error {
      DBError::Database(err) => {
        let msg = err.to_string();
        ServiceError::BadRequest(msg)
      }
      _ => ServiceError::InternalServerError,
    }
  }
}

pub type ServiceResult<V> = std::result::Result<V, ServiceError>;
