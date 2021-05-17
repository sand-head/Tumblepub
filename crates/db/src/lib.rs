use anyhow::Result;
use sqlx::{Connection, PgConnection, Row};
use url::Url;

pub mod models;

pub async fn create_database_if_not_exists(db_url: &str) -> Result<()> {
  // todo: do this better now that the Options struct exists
  let db_url = Url::parse(db_url).expect("Could not parse DATABASE_URL");
  let db_name = &db_url.path().clone()[1..];
  let mut base_url = db_url.clone();
  base_url
    .path_segments_mut()
    .expect("Could not get path segments")
    .clear()
    .push("postgres");

  let mut conn = PgConnection::connect(base_url.as_str()).await?;
  let row = sqlx::query(&format!(
    "SELECT EXISTS(SELECT datname FROM pg_catalog.pg_database WHERE datname = '{}') AS exists",
    db_name
  ))
  .fetch_one(&mut conn)
  .await?;

  if !row.try_get("exists")? {
    let result = sqlx::query(&format!("CREATE DATABASE {}", db_name))
      .execute(&mut conn)
      .await;
    match result {
      Ok(_) => println!("Database {} created!", db_name),
      Err(_) => panic!("Could not create database"),
    }
  }

  Ok(())
}
