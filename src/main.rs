#[macro_use]
extern crate diesel;
#[macro_use]
extern crate diesel_migrations;

use std::sync::Arc;

use actix_files as fs;
use actix_web::{middleware::Logger, web, App, HttpServer};
use env::load_dotenv;

mod database;
mod env;
mod errors;
mod graphql;
mod routes;
mod theme;

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

  println!("Establishing database pool...");
  database::create_database_if_not_exists().await;
  let pool = database::establish_pool();
  println!("Running migrations...");
  database::run_migrations(&pool);

  println!("Creating GraphQL schema...");
  let schema = Arc::new(graphql::create_schema());

  println!("Starting webserver...");
  let server = HttpServer::new(move || {
    let schema: web::Data<graphql::Schema> = schema.clone().into();
    App::new()
      // Database:
      .data(pool.clone())
      .app_data(schema)
      // Error logging:
      .wrap(Logger::default())
      // ActivityPub services:
      .service(routes::well_known::webfinger)
      // GraphQL:
      .configure(routes::graphql::routes)
      // Other services:
      // .service(routes::hello)
      // Static files:
      .service(fs::Files::new("/", "./build").index_file("index.html"))
      .default_service(web::resource("").route(web::get().to(routes::index)))
  })
  .bind("127.0.0.1:8080")?
  .run();

  println!("🚀 Listening on http://127.0.0.1:8080!");
  server.await
}
