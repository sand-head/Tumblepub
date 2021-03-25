use anyhow::Result;
use chrono::NaiveDateTime;
use sqlx::{PgConnection, PgPool};

#[derive(Debug)]
pub struct Blog {
  pub id: i64,
  pub uri: Option<String>,
  pub name: String,
  pub domain: Option<String>,
  pub is_public: bool,
  pub title: Option<String>,
  pub description: Option<String>,
  pub created_at: NaiveDateTime,
  pub updated_at: NaiveDateTime,
}

pub struct InsertableBlog {
  pub uri: Option<String>,
  pub name: String,
  pub domain: Option<String>,
  pub is_public: bool,
  pub title: Option<String>,
  pub description: Option<String>,
}

impl Blog {
  /// Creates a new Blog and returns it
  pub async fn create(conn: &mut PgConnection, model: InsertableBlog) -> Result<Self> {
    Ok(
      sqlx::query_as!(
        Blog,
        r#"
INSERT INTO blogs (uri, name, domain, is_public, title, description)
VALUES ($1, $2, $3, $4, $5, $6)
RETURNING *
        "#,
        model.uri,
        model.name,
        model.domain,
        model.is_public,
        model.title,
        model.description
      )
      .fetch_one(conn)
      .await?,
    )
  }

  /// Searches for a Blog by the given name & domain tuple and returns it if found.
  pub async fn find(pool: &PgPool, name: (String, Option<String>)) -> Result<Option<Self>> {
    let (name, domain) = name;

    if let Some(domain) = domain {
      Ok(
        sqlx::query_as!(
          Blog,
          "SELECT * FROM blogs WHERE name = $1 AND domain = $2 LIMIT 1",
          name,
          domain
        )
        .fetch_optional(pool)
        .await?,
      )
    } else {
      Ok(
        sqlx::query_as!(
          Blog,
          "SELECT * FROM blogs WHERE name = $1 AND domain IS NULL LIMIT 1",
          name
        )
        .fetch_optional(pool)
        .await?,
      )
    }
  }

  pub async fn list(pool: &PgPool, limit: Option<i32>, offset: Option<i32>) -> Result<Vec<Self>> {
    let limit = limit.unwrap_or(50) as i64;
    let offset = offset.unwrap_or(0) as i64;

    Ok(
      sqlx::query_as!(
        Blog,
        "SELECT * FROM blogs LIMIT $1 OFFSET $2",
        limit,
        offset
      )
      .fetch_all(pool)
      .await?,
    )
  }
}
