use anyhow::Result;
use chrono::{DateTime, Utc};
use sqlx::types::Json;
use sqlx::PgConnection;
use uuid::Uuid;

use super::post::Post;
use crate::models::post::PostContent;

pub type BlogName = (String, Option<String>);

#[derive(Debug, Clone)]
pub struct Blog {
  pub id: i64,
  pub user_id: i64,
  pub uri: Option<String>,
  pub name: String,
  pub domain: Option<String>,
  pub is_primary: bool,
  pub is_public: bool,
  pub is_nsfw: bool,
  pub title: Option<String>,
  pub description: Option<String>,
  pub theme_id: Option<Uuid>,
  pub private_key: Option<Vec<u8>>,
  pub public_key: Vec<u8>,
  pub created_at: DateTime<Utc>,
  pub updated_at: DateTime<Utc>,
}

#[derive(Debug)]
pub struct BlogTheme {
  pub id: Uuid,
  pub hash: String,
  pub theme: String,
  pub created_at: DateTime<Utc>,
}

pub struct NewBlog {
  pub user_id: i64,
  pub uri: Option<String>,
  pub name: String,
  pub domain: Option<String>,
  pub is_primary: bool,
  pub is_public: bool,
  pub title: Option<String>,
  pub description: Option<String>,
  pub private_key: Vec<u8>,
  pub public_key: Vec<u8>,
}

impl Blog {
  /// Creates a new Blog and returns it
  pub async fn create_new(conn: &mut PgConnection, model: NewBlog) -> Result<Self> {
    Ok(
      sqlx::query_as!(
        Blog,
        r#"
INSERT INTO blogs (user_id, uri, name, domain, is_primary, is_public, title, description, private_key, public_key)
VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10)
RETURNING *
        "#,
        model.user_id,
        model.uri,
        model.name,
        model.domain,
        model.is_primary,
        model.is_public,
        model.title,
        model.description,
        model.private_key,
        model.public_key
      )
      .fetch_one(conn)
      .await?,
    )
  }

  /// Searches for a Blog by the given name & domain tuple and returns it if found.
  pub async fn find(conn: &mut PgConnection, name: BlogName) -> Result<Option<Self>> {
    let (name, domain) = name;

    if let Some(domain) = domain {
      Ok(
        sqlx::query_as!(
          Blog,
          "SELECT * FROM blogs WHERE name = $1 AND domain = $2 LIMIT 1",
          name,
          domain
        )
        .fetch_optional(conn)
        .await?,
      )
    } else {
      Ok(
        sqlx::query_as!(
          Blog,
          "SELECT * FROM blogs WHERE name = $1 AND domain IS NULL LIMIT 1",
          name
        )
        .fetch_optional(conn)
        .await?,
      )
    }
  }

  pub async fn get_by_id(conn: &mut PgConnection, id: i64) -> Result<Option<Self>> {
    Ok(
      sqlx::query_as!(Blog, "SELECT * FROM blogs WHERE id = $1", id)
        .fetch_optional(conn)
        .await?,
    )
  }

  pub async fn list(
    conn: &mut PgConnection,
    limit: Option<i32>,
    offset: Option<i32>,
  ) -> Result<Vec<Self>> {
    let limit = limit.unwrap_or(50) as i64;
    let offset = offset.unwrap_or(0) as i64;

    Ok(
      sqlx::query_as!(
        Blog,
        "SELECT * FROM blogs LIMIT $1 OFFSET $2",
        limit,
        offset
      )
      .fetch_all(conn)
      .await?,
    )
  }

  pub async fn total_posts(&self, conn: &mut PgConnection) -> Result<usize> {
    Ok(
      sqlx::query!(
        r#"
SELECT COUNT(id) as count
FROM posts
WHERE blog_id = $1
        "#,
        &self.id,
      )
      .fetch_all(conn)
      .await?[0]
        .count
        .unwrap_or(0) as usize,
    )
  }

  pub async fn posts(
    &self,
    conn: &mut PgConnection,
    limit: Option<i32>,
    offset: Option<i32>,
  ) -> Result<Vec<Post>> {
    let limit = limit.unwrap_or(25) as i64;
    let offset = offset.unwrap_or(0) as i64;

    Ok(
      sqlx::query_as!(
        Post,
        r#"
SELECT id, blog_id, content as "content: Json<Vec<PostContent>>", created_at, updated_at
FROM posts
WHERE blog_id = $1
ORDER BY created_at DESC
LIMIT $2
OFFSET $3
        "#,
        &self.id,
        limit,
        offset
      )
      .fetch_all(conn)
      .await?,
    )
  }

  pub async fn theme(&self, conn: &mut PgConnection) -> Result<Option<BlogTheme>> {
    Ok(match self.theme_id {
      Some(id) => Some(
        sqlx::query_as!(BlogTheme, "SELECT * FROM blog_themes WHERE id = $1", id)
          .fetch_one(conn)
          .await?,
      ),
      None => None,
    })
  }

  pub async fn set_description(&mut self, conn: &mut PgConnection, desc: String) -> Result<()> {
    let result = sqlx::query!(
      r#"
UPDATE blogs
SET description = $1
WHERE id = $2
      "#,
      desc,
      self.id
    )
    .execute(conn)
    .await;

    match result {
      Ok(_) => {
        self.description = Some(desc);
        Ok(())
      }
      Err(e) => Err(e.into()),
    }
  }
}
