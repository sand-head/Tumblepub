use std::env;

use diesel::{r2d2, sql_types::Bool, Connection, PgConnection, RunQueryDsl};

use crate::errors::{ServiceError, ServiceResult};

pub mod models;
pub mod schema;

type ConnectionManager = r2d2::ConnectionManager<PgConnection>;
pub type DbPool = r2d2::Pool<ConnectionManager>;
pub type DbConnection = r2d2::PooledConnection<ConnectionManager>;

embed_migrations!();

#[derive(QueryableByName)]
struct DbExistResult {
  #[sql_type = "Bool"]
  exists: bool,
}

pub async fn create_database_if_not_exists() {
  let db_user = env::var("DB_USERNAME").expect("Environment variable DB_USERNAME must be set");
  let db_pass = env::var("DB_PASSWORD").expect("Environment variable DB_PASSWORD must be set");
  let db_host = env::var("DB_HOST").expect("Environment variable DB_HOST must be set");
  let db_port = env::var("DB_PORT").expect("Environment variable DB_PORT must be set");
  let db_name = env::var("DB_NAME").expect("Environment variable DB_NAME must be set");

  let db_url = format!(
    "postgres://{}:{}@{}:{}/postgres",
    db_user, db_pass, db_host, db_port
  );
  let conn = PgConnection::establish(&db_url).expect("Could not connect to PostgreSQL database");

  let query = format!(
    "SELECT EXISTS(SELECT datname FROM pg_catalog.pg_database WHERE datname = '{}') AS exists",
    db_name
  );
  let result = diesel::sql_query(query)
    .get_result::<DbExistResult>(&conn)
    .expect("Could not retrieve database creation status from PostgreSQL");

  if !result.exists {
    match diesel::sql_query(&format!("CREATE DATABASE {}", db_name)).execute(&conn) {
      Ok(_) => println!("Database {} created!", db_name),
      Err(_) => panic!("Could not create database"),
    }
  }
}

pub fn establish_pool() -> DbPool {
  let db_user = env::var("DB_USERNAME").expect("Environment variable DB_USERNAME must be set");
  let db_pass = env::var("DB_PASSWORD").expect("Environment variable DB_PASSWORD must be set");
  let db_host = env::var("DB_HOST").expect("Environment variable DB_HOST must be set");
  let db_port = env::var("DB_PORT").expect("Environment variable DB_PORT must be set");
  let db_name = env::var("DB_NAME").expect("Environment variable DB_NAME must be set");

  let db_url = format!(
    "postgres://{}:{}@{}:{}/{}",
    db_user, db_pass, db_host, db_port, db_name
  );
  let manager = ConnectionManager::new(db_url);
  let pool = DbPool::builder().build(manager);
  match pool {
    Ok(pool) => pool,
    Err(err) => {
      panic!("An error occurred: {}", err);
    }
  }
}

pub fn run_migrations(pool: &DbPool) {
  let connection = pool.get().expect("Could not retrieve connection from pool");
  embedded_migrations::run(&connection).expect("Could not run migrations");
}

pub fn connect(pool: &DbPool) -> ServiceResult<DbConnection> {
  Ok(pool.get().map_err(|_| ServiceError::DbConnectionError)?)
}
