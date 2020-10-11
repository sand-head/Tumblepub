#[macro_use]
extern crate diesel;
#[macro_use]
extern crate diesel_migrations;

use actix_files as fs;
use actix_web::{middleware::Logger, web, App, HttpServer};
use env::load_dotenv;
use handlebars::Handlebars;

mod database;
mod env;
pub mod models;
mod routes;
pub mod schema;
mod theme;

pub struct AppState<'hbs> {
  hbs: Handlebars<'hbs>,
  pool: database::DbPool,
}

pub fn get_data(content: String) -> theme::ThemeVariables {
  theme::ThemeVariables {
    title: "bloq".to_string(),
    description: None,
    content,

    previous_page: None,
    next_page: None,
  }
}

#[actix_web::main]
async fn main() -> std::io::Result<()> {
  println!("Loading environment variables...");
  load_dotenv();
  database::create_database_if_not_exists().await;
  println!("Establishing database pool...");
  let pool = database::establish_pool();
  println!("Running migrations...");
  database::run_migrations(&pool);

  let state = web::Data::new(AppState {
    hbs: theme::create_handlebars(),
    pool: pool.clone(),
  });

  println!("Starting webserver on port 8080...");
  HttpServer::new(move || {
    App::new()
      .app_data(state.clone())
      .wrap(Logger::default())
      // ActivityPub services:
      .service(routes::well_known::webfinger)
      // Other services:
      // .service(routes::hello)
      // Static files
      .service(fs::Files::new("/", "./build").index_file("index.html"))
      .default_service(web::resource("").route(web::get().to(routes::index)))
  })
  .bind("127.0.0.1:8080")?
  .run()
  .await
}
