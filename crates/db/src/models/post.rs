use chrono::NaiveDateTime;
use serde::{Deserialize, Serialize};
use sqlx::types::{Json, Uuid};

pub struct Post {
  pub id: Uuid,
  pub blog_id: i64,
  pub content: Json<Vec<PostContent>>,
  pub created_at: NaiveDateTime,
  pub updated_at: NaiveDateTime,
}

#[derive(Debug, Deserialize, Serialize)]
pub enum PostContent {
  Markdown(String),
}
