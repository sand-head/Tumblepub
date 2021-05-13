use actix_web::web;
use async_graphql_actix_web::{Request, Response};

use tumblepub_gql::TumblepubSchema;

async fn graphql(schema: web::Data<TumblepubSchema>, req: Request) -> Response {
  schema.execute(req.into_inner()).await.into()
}

pub fn routes(config: &mut web::ServiceConfig) {
  config.service(web::resource("/graphql").route(web::post().to(graphql)));
}
