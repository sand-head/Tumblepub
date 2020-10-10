use actix_web::{get, web, HttpResponse, Responder};

use crate::{AppState, get_data};

#[get("/")]
pub async fn hello(data: web::Data<AppState<'_>>) -> impl Responder {
  let body = data.hbs.render("index", &get_data("hello world!".to_string())).unwrap();
  HttpResponse::Ok().body(body)
}

pub async fn not_found(data: web::Data<AppState<'_>>) -> impl Responder {
  let body = data.hbs.render("index", &get_data("not found :(".to_string())).unwrap();
  HttpResponse::NotFound().body(body)
}