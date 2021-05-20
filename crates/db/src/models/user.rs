use anyhow::Result;
use argon2rs::argon2i_simple;
use chrono::{DateTime, Utc};
use sqlx::PgConnection;

use tumblepub_utils::jwt::Token;

use super::blog::Blog;

/// Represents a user's data and settings.
#[derive(Debug)]
pub struct User {
  pub id: i64,
  pub email: String,
  pub primary_blog: i64,
  pub hash: Vec<u8>,
  pub salt: String,
  pub created_at: DateTime<Utc>,
  pub updated_at: DateTime<Utc>,
  pub last_login_at: DateTime<Utc>,
}

#[derive(Debug)]
pub struct NewUser {
  pub email: String,
  pub password: String,
  pub primary_blog: i64,
}

impl User {
  pub async fn count(conn: &mut PgConnection) -> Result<i64> {
    let record = sqlx::query!(r#"SELECT COUNT(*) as "count!" FROM users"#)
      .fetch_one(conn)
      .await?;

    Ok(record.count)
  }

  /// Creates a new user and returns it
  pub async fn create(conn: &mut PgConnection, model: NewUser) -> Result<User> {
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

  /// Returns a user if and only if the provided credentials are valid
  pub async fn verify(
    conn: &mut PgConnection,
    email: String,
    password: String,
  ) -> Result<Option<User>> {
    let result = sqlx::query_as!(User, "SELECT * FROM users WHERE email = $1 LIMIT 1", email)
      .fetch_optional(conn)
      .await?;

    if let Some(user) = result {
      let incoming_hash = argon2i_simple(&password, &user.salt);
      if incoming_hash == user.hash.as_ref() {
        return Ok(Some(user));
      }
    }

    Ok(None)
  }

  /// Gets a user by their ID, or `Option::None` if none exists.
  pub async fn get_by_id(conn: &mut PgConnection, id: i64) -> Result<Option<Self>> {
    Ok(
      sqlx::query_as!(User, "SELECT * FROM users WHERE id = $1 LIMIT 1", id)
        .fetch_optional(conn)
        .await?,
    )
  }

  /// Gets a user by the claims in a `Token`, or `Option::None` if none exists.
  pub async fn get_by_token(conn: &mut PgConnection, token: &Token) -> Result<Option<Self>> {
    let claims = token.get_claims();
    match claims {
      None => Err(anyhow::anyhow!("could not get claims from token")),
      Some(claims) => User::get_by_id(conn, claims.sub).await,
    }
  }

  /// Returns the user's primary blog from the database.
  pub async fn primary_blog(&self, conn: &mut PgConnection) -> Result<Blog> {
    Ok(
      sqlx::query_as!(
        Blog,
        "SELECT * FROM blogs WHERE id = $1 LIMIT 1",
        self.primary_blog
      )
      .fetch_one(conn)
      .await?,
    )
  }
}
