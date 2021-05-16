use actix_web::{web, HttpRequest};
use async_graphql_actix_web::{Request, Response};

use tumblepub_gql::TumblepubSchema;
use tumblepub_utils::jwt::Token;

async fn graphql(
  schema: web::Data<TumblepubSchema>,
  req: HttpRequest,
  gql_req: Request,
) -> Response {
  // extract JWT as String from headers
  let token = req
    .headers()
    .get("Token")
    .and_then(|value| value.to_str().map(|str| String::from(str)).ok())
    .unwrap_or_else(|| String::new());
  let token = Token::from(token);

  schema
    .execute(gql_req.into_inner().data(token))
    .await
    .into()
}

pub fn routes(config: &mut web::ServiceConfig) {
  config.service(web::resource("/graphql").route(web::post().to(graphql)));
}
