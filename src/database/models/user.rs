use chrono::NaiveDateTime;
use sqlx::PgPool;

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
  pub primary_blog: i64,
  pub hash: Vec<u8>,
  pub salt: String,
}

impl User {
  pub async fn create(pool: &PgPool, model: InsertableUser) -> ServiceResult<Self> {
    // todo: also create a blog & user_blog
    // actually, is a user_blog entry required here?
    // do some research, maybe you can't have more than one person behind a primary blog
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
        model.hash,
        model.salt,
      )
      .fetch_one(pool)
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
