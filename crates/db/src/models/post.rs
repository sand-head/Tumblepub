use anyhow::Result;
use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};
use sqlx::{
  types::{Json, Uuid},
  PgConnection,
};

#[derive(Debug)]
pub struct Post {
  pub id: Uuid,
  pub blog_id: i64,
  pub content: Json<Vec<PostContent>>,
  pub created_at: DateTime<Utc>,
  pub updated_at: DateTime<Utc>,
}

#[derive(Debug, Deserialize, Serialize)]
pub enum PostContent {
  Markdown(String),
}

pub struct NewPost {
  pub blog_id: i64,
  pub content: Vec<PostContent>,
}

impl Post {
  pub async fn create_new(conn: &mut PgConnection, model: NewPost) -> Result<Self> {
    Ok(
      sqlx::query_as!(
        Post,
        r#"
INSERT INTO posts (blog_id, content)
VALUES ($1, $2)
RETURNING id, blog_id, content as "content: Json<Vec<PostContent>>", created_at, updated_at
        "#,
        model.blog_id,
        Json(model.content) as _
      )
      .fetch_one(conn)
      .await?,
    )
  }
  pub async fn find(conn: &mut PgConnection, id: Uuid) -> Result<Option<Self>> {
    Ok(
      sqlx::query_as!(
        Post,
        r#"
SELECT id, blog_id, content as "content: Json<Vec<PostContent>>", created_at, updated_at
FROM posts
WHERE id = $1
LIMIT 1
        "#,
        id
      )
      .fetch_optional(conn)
      .await?,
    )
  }
}
