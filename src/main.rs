use std::sync::Arc;

use actix_web::{middleware::Logger, web, App, HttpServer};
use anyhow::Result;
use env::load_dotenv;
use once_cell::sync::Lazy;
use sqlx::PgPool;
use tumblepub_gql::{create_schema, TumblepubSchema};

mod env;
mod routes;
mod theme;

static LOCAL_DOMAIN: Lazy<String> = Lazy::new(|| {
  std::env::var("LOCAL_DOMAIN").expect("Environment variable LOCAL_DOMAIN must be set.")
});

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
  let schema = Arc::new(create_schema(pool.clone()));

  println!("Starting webserver...");
  let server = HttpServer::new(move || {
    let schema: web::Data<TumblepubSchema> = schema.clone().into();
    App::new()
      // Database:
      .data(pool.clone())
      .app_data(schema)
      // Error logging:
      .wrap(Logger::default())
      // ActivityPub services:
      .configure(routes::well_known::routes)
      .configure(routes::activitypub::routes)
      // GraphQL:
      .configure(routes::graphql::routes)
      // Blog content:
      .configure(routes::blog::routes)
  })
  .bind("0.0.0.0:8080")?
  .run();

  println!("🚀 Listening on http://0.0.0.0:8080");
  server.await?;

  Ok(())
}
