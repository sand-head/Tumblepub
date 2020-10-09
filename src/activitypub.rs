use actix_web::{HttpResponse, Responder, get};

#[get("/.well-known/webfinger")]
pub async fn webfinger() -> impl Responder {
  HttpResponse::Ok()
}

fn inbox() {}

fn outbox() {}

