use argon2rs::argon2i_simple;
use chrono::NaiveDateTime;
use sqlx::{PgConnection, PgPool};

use crate::database::models::blog::Blog;
use crate::errors::ServiceResult;

/// Represents a user's data and settings.
#[derive(Debug)]
pub struct User {
  pub id: i64,
  pub email: String,
  pub primary_blog: i64,
  pub hash: Vec<u8>,
  pub salt: String,
  pub created_at: NaiveDateTime,
  pub updated_at: NaiveDateTime,
  pub last_login_at: NaiveDateTime,
}

#[derive(Debug)]
pub struct InsertableUser {
  pub email: String,
  pub password: String,
  pub primary_blog: i64,
}

impl User {
  /// Creates a new user and returns it
  pub async fn create(conn: &mut PgConnection, model: InsertableUser) -> ServiceResult<User> {
    // step 1: we salt our hashbrowns
    let salt: String = {
      use rand::Rng;
      const CHARSET: &[u8] =
        b"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789)(*&^%$#@!~";
      const PASSWORD_LEN: usize = 128;
      let mut rng = rand::thread_rng();

      (0..PASSWORD_LEN)
        .map(|_| {
          let index = rng.gen_range(0..CHARSET.len());
          CHARSET[index] as char
        })
        .collect()
    };
    let hash = argon2i_simple(&model.password, &salt).to_vec();

    // step 2: we create a new user
    Ok(
      sqlx::query_as!(
        User,
        r#"
INSERT INTO users (email, primary_blog, hash, salt)
VALUES ($1, $2, $3, $4)
RETURNING *
      "#,
        model.email,
        model.primary_blog,
        hash,
        salt,
      )
      .fetch_one(conn)
      .await?,
    )
  }

  pub async fn find(pool: &PgPool, user_id: i64) -> ServiceResult<Option<Self>> {
    Ok(
      sqlx::query_as!(User, "SELECT * FROM users WHERE id = $1 LIMIT 1", user_id)
        .fetch_optional(pool)
        .await?,
    )
  }
}
