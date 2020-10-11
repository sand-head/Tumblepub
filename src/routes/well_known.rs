use actix_web::{get, web, HttpResponse, Responder};
use serde::{Deserialize, Serialize};

#[derive(Deserialize)]
pub struct WebfingerReq {
  resource: Option<String>,
}

#[derive(Serialize)]
pub enum WebfingerLink {}

#[derive(Serialize)]
pub struct WebfingerRes {
  subject: String,
  aliases: Vec<String>,
  links: Vec<WebfingerLink>,
}

#[get("/.well-known/webfinger")]
pub async fn webfinger(query: web::Query<WebfingerReq>) -> impl Responder {
  match &query.resource {
    Some(resource) => {
      println!("requested resource: {}", resource);

      // todo: get from database
      let res = WebfingerRes {
        subject: resource.clone(),
        aliases: vec![],
        links: vec![],
      };

      HttpResponse::Ok()
        .content_type("application/activity+json")
        .json(res)
    }
    None => HttpResponse::NotFound().finish(),
  }
}
