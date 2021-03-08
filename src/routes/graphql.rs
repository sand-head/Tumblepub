use actix_web::{web, Error, HttpResponse};
use juniper_actix::graphql_handler;
use sqlx::PgPool;

use crate::graphql::{Context, Schema};

async fn graphql(
  schema: web::Data<Schema>,
  pool: web::Data<PgPool>,
  req: web::HttpRequest,
  payload: web::Payload,
) -> Result<HttpResponse, Error> {
  let context = Context::new(pool.into_inner().as_ref().clone());

  graphql_handler(&schema, &context, req, payload).await
}

pub fn routes(config: &mut web::ServiceConfig) {
  config.service(web::resource("/graphql").route(web::post().to(graphql)));
}
