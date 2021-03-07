use actix_web::{error, web, Error, HttpResponse};
use juniper::http::GraphQLRequest;

use crate::{
  database::DbPool,
  graphql::{Context, Schema},
};

pub async fn graphql(
  schema: web::Data<Schema>,
  data: web::Json<GraphQLRequest>,
  pool: web::Data<DbPool>,
) -> Result<HttpResponse, Error> {
  let context = Context::new(pool.into_inner().as_ref().clone());
  let response = data.execute(&schema, &context).await;
  let json = serde_json::to_string(&response).map_err(error::ErrorInternalServerError)?;

  Ok(HttpResponse::Ok().json(json))
}
