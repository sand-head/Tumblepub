use std::fmt::Result;

use std::sync::Arc;

use actix_web::{middleware::Logger, web, App, HttpServer};
use anyhow::Result;
use sqlx::PgPool;
use tumblepub_events::models::user::User;
use tumblepub_gql::{create_schema, mutation::register::register, TumblepubSchema};
use tumblepub_utils::options::Options;

mod routes;
mod theme;

async fn single_user_init(pool: &PgPool) -> Result<()> {
  let mut conn = pool.acquire().await.unwrap();
  if Options::get().single_user_mode && User::count(&mut conn).await? == 0 {
    // there are no users, so let's create the provided single user
    let single_user = Options::get().single_user.ok_or_else(|| {
      anyhow::anyhow!(r#"could not get "single_user" options, please set them before running"#)
    })?;
    register(
      pool,
      single_user.email,
      single_user.password,
      single_user.username.to_owned(),
    )
    .await
    .map_err(|_| anyhow::anyhow!("could not register single user"))?;
    println!("Created user {:?}!", single_user.username);
  }
  Ok(())
}

pub async fn run() -> Result<()> {
  println!("Establishing database pool...");
  tumblepub_events::create_database_if_not_exists().await?;
  let db_url = Options::get().database.database_url(None);
  let pool = PgPool::connect(&db_url).await?;

  println!("Running migrations and performing database setup...");
  sqlx::migrate!("./migrations").run(&pool).await?;
  single_user_init(&pool).await?;

  println!("Creating GraphQL schema...");
  let schema = Arc::new(create_schema(pool.clone()));

  println!("Starting webserver...");
  // access logs are printed with the INFO level so ensure it is enabled by default
  env_logger::init_from_env(env_logger::Env::new().default_filter_or("info"));

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
      // Assets:
      .configure(routes::assets::routes)
      // Blog content:
      .configure(routes::blog::routes)
  })
  .bind("0.0.0.0:8080")?
  .run();

  println!("🚀 Listening on http://0.0.0.0:8080");
  server.await?;

  Ok(())
}
