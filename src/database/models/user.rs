use argon2rs::argon2i_simple;
use chrono::NaiveDateTime;
use sqlx::PgPool;

use crate::database::models::blog::Blog;
use crate::errors::{ServiceError, ServiceResult};

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
  pub name: String,
}

impl User {
  /// Creates a new user alongside their primary blog, and returns a tuple of them both
  pub async fn create(pool: &PgPool, model: InsertableUser) -> ServiceResult<(User, Blog)> {
    let mut transaction = pool.begin().await?;
    // step 1: create a primary blog for the new user
    // todo: maybe we can delegate this to Blog::create
    let result = sqlx::query_as!(
      Blog,
      r#"
INSERT INTO blogs (uri, name, domain, title, description)
VALUES ($1, $2, $3, $4, $5)
RETURNING *
      "#,
      Option::<String>::None,
      model.name,
      Option::<String>::None,
      Option::<String>::None,
      Option::<String>::None
    )
    .fetch_one(&mut transaction)
    .await;

    match result {
      Ok(primary_blog) => {
        // step 2: we salt our hashbrowns
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

        // step 3: we (finally) create a new user
        let result = sqlx::query_as!(
          User,
          r#"
INSERT INTO users (email, primary_blog, hash, salt)
VALUES ($1, $2, $3, $4)
RETURNING *
          "#,
          model.email,
          primary_blog.id,
          hash,
          salt,
        )
        .fetch_one(&mut transaction)
        .await;

        // if everything's gone smoothly, commit the transaction
        match result {
          Ok(user) => {
            transaction.commit().await?;
            Ok((user, primary_blog))
          }
          Err(_) => {
            transaction.rollback().await?;
            Err(ServiceError::BadRequest(
              "Could not create user".to_string(),
            ))
          }
        }
      }
      Err(_) => {
        transaction.rollback().await?;
        Err(ServiceError::BadRequest(
          "Could not create primary blog".to_string(),
        ))
      }
    }
  }

  pub async fn find(pool: &PgPool, user_id: i64) -> ServiceResult<Option<Self>> {
    Ok(
      sqlx::query_as!(User, "SELECT * FROM users WHERE id = $1 LIMIT 1", user_id)
        .fetch_optional(pool)
        .await?,
    )
  }
}
