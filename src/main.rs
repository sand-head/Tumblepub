#[macro_use]
extern crate diesel;
#[macro_use]
extern crate diesel_migrations;

use activitypub::webfinger;
use actix_web::{get, web, App, HttpResponse, HttpServer, Responder};
use database::establish_pool;
use diesel::{PgConnection, r2d2};
use env::load_dotenv;
use handlebars::Handlebars;

mod activitypub;
mod database;
mod env;
pub mod models;
pub mod schema;
mod theme;

type DbPool = r2d2::Pool<r2d2::ConnectionManager<PgConnection>>;

struct AppState<'hbs> {
  hbs: Handlebars<'hbs>,
  pool: DbPool
}

embed_migrations!();

fn run_migrations(pool: &DbPool) {
  let connection = pool.get()
    .expect("Could not retrieve connection from pool");
  embedded_migrations::run(&connection)
    .expect("Could not run migrations");
}

fn get_data(content: String) -> theme::ThemeVariables {
  theme::ThemeVariables {
    title: "schweigert.dev".to_string(),
    description: None,
    content,

    previous_page: None,
    next_page: None
  }
}

#[get("/")]
async fn hello(data: web::Data<AppState<'_>>) -> impl Responder {
  let body = data.hbs.render("index", &get_data("hello world!".to_string())).unwrap();
  HttpResponse::Ok().body(body)
}

async fn not_found(data: web::Data<AppState<'_>>) -> impl Responder {
  let body = data.hbs.render("index", &get_data("not found :(".to_string())).unwrap();
  HttpResponse::NotFound().body(body)
}

#[actix_web::main]
async fn main() -> std::io::Result<()> {
  load_dotenv();
  let pool = establish_pool();
  run_migrations(&pool);

  let mut registry = Handlebars::new();
  registry.register_template_file("index", "./templates/index.hbs")
    .expect("./templates/index.hbs not found");

  let state = web::Data::new(AppState {
    hbs: registry,
    pool: pool.clone()
  });

  HttpServer::new(move || {
    App::new()
      .app_data(state.clone())
      // ActivityPub services:
      .service(webfinger)
      // Other services:
      .service(hello)
      .default_service(web::route().to(not_found))
  })
    .bind("127.0.0.1:8080")?
    .run()
    .await
}
