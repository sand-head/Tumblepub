use diesel::{pg::PgConnection, r2d2};
use std::env;

pub fn establish_pool() -> r2d2::Pool<r2d2::ConnectionManager<PgConnection>> {
  let db_url = env::var("DATABASE_URL")
    .expect("DATABASE_URL must be set");
    
  let manager = r2d2::ConnectionManager::<PgConnection>::new(db_url);
  let pool = r2d2::Pool::builder()
    .build(manager)
    .expect("Failed to create pool");
  pool
}