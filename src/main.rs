use std::sync::Arc;

use actix_files as fs;
use actix_web::{middleware::Logger, web, App, HttpServer};
use anyhow::Result;
use env::load_dotenv;
use lazy_static::lazy_static;
use sqlx::PgPool;

mod env;
mod errors;
mod graphql;
mod routes;
mod theme;

lazy_static! {
  pub static ref LOCAL_DOMAIN: String =
    std::env::var("LOCAL_DOMAIN").expect("Environment variable LOCAL_DOMAIN must be set.");
}

pub fn get_data(content: String) -> theme::ThemeVariables {
  theme::ThemeVariables {
    title: "tumblepub".to_string(),
    description: None,
    content,

    previous_page: None,
    next_page: None,
  }
}

#[actix_web::main]
async fn main() -> Result<()> {
  println!("Loading environment variables...");
  load_dotenv();
  let db_url = std::env::var("DATABASE_URL")?;

  println!("Establishing database pool...");
  tumblepub_db::create_database_if_not_exists(&db_url).await?;
  let pool = PgPool::connect(&db_url).await?;
  println!("Running migrations...");
  sqlx::migrate!("./migrations").run(&pool).await?;

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
      .configure(routes::well_known::routes)
      // GraphQL:
      .configure(routes::graphql::routes)
      // Blog content:
      .configure(routes::blog::routes)
  })
  .bind("127.0.0.1:8080")?
  .run();

  println!("🚀 Listening on http://127.0.0.1:8080");
  server.await?;

  Ok(())
}
