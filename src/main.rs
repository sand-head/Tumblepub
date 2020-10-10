#[macro_use]
extern crate diesel;
#[macro_use]
extern crate diesel_migrations;

use activitypub::webfinger;
use actix_web::{web, App, HttpServer};
use env::load_dotenv;
use handlebars::Handlebars;

mod activitypub;
mod database;
mod env;
pub mod models;
mod routes;
pub mod schema;
mod theme;

pub struct AppState<'hbs> {
  hbs: Handlebars<'hbs>,
  pool: database::DbPool
}

pub fn get_data(content: String) -> theme::ThemeVariables {
  theme::ThemeVariables {
    title: "bloq".to_string(),
    description: None,
    content,

    previous_page: None,
    next_page: None
  }
}

#[actix_web::main]
async fn main() -> std::io::Result<()> {
  load_dotenv();
  let pool = database::establish_pool();
  database::run_migrations(&pool);

  let state = web::Data::new(AppState {
    hbs: theme::create_handlebars(),
    pool: pool.clone()
  });

  HttpServer::new(move || {
    App::new()
      .app_data(state.clone())
      // ActivityPub services:
      .service(webfinger)
      // Other services:
      .service(routes::hello)
      .default_service(web::route().to(routes::not_found))
  })
    .bind("127.0.0.1:8080")?
    .run()
    .await
}
