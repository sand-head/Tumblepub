use anyhow::Result;
use sqlx::{Connection, PgConnection};
use tumblepub_utils::options::Options;

pub mod models;

pub async fn create_database_if_not_exists() -> Result<()> {
  let db_name = Options::get().database().db_name();
  let postgres_url = Options::get()
    .database()
    .database_url(Some("postgres".to_string()));

  let mut conn = PgConnection::connect(&postgres_url).await?;
  let row = sqlx::query!(
    r#"SELECT EXISTS(SELECT datname FROM pg_catalog.pg_database WHERE datname = $1) AS "exists!""#,
    db_name
  )
  .fetch_one(&mut conn)
  .await?;

  if !row.exists {
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
