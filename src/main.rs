use actix_web::{get, web, App, HttpResponse, HttpServer, Responder};
use handlebars::Handlebars;
use serde_json::json;

struct AppState<'hbs> {
  hbs: Handlebars<'hbs>
}

fn get_data(content: &str) -> serde_json::Value {
  json![{
    "Title": "schweigert.dev",
    "Content": content
  }]
}

#[get("/")]
async fn hello(data: web::Data<AppState<'_>>) -> impl Responder {
  let body = data.hbs.render("index", &get_data("hello world!")).unwrap();
  HttpResponse::Ok().body(body)
}

async fn not_found(data: web::Data<AppState<'_>>) -> impl Responder {
  let body = data.hbs.render("index", &get_data("not found :(")).unwrap();
  HttpResponse::NotFound().body(body)
}

#[actix_web::main]
async fn main() -> std::io::Result<()> {
  let mut registry = Handlebars::new();
  registry.register_template_file("index", "./templates/index.hbs")
    .expect("./templates/index.hbs not found");
  let state = web::Data::new(AppState {
    hbs: registry
  });

  HttpServer::new(move || {
    App::new()
      .app_data(state.clone())
      .service(hello)
      .default_service(web::route().to(not_found))
  })
    .bind("127.0.0.1:8080")?
    .run()
    .await
}
