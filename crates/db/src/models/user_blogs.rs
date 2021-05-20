use anyhow::Result;
use sqlx::PgConnection;

/// Represents a many-to-many relationship between users and blogs.
#[derive(sqlx::FromRow, Debug)]
pub struct UserBlogs {
  pub user_id: i64,
  pub blog_id: i64,
  pub is_admin: bool,
}
impl UserBlogs {
  pub async fn exists(conn: &mut PgConnection, user_id: i64, blog_id: i64) -> Result<bool> {
    let record = sqlx::query!(
      r#"
SELECT EXISTS(SELECT 1 FROM user_blogs WHERE user_id = $1 AND blog_id = $2) AS "exists!"
      "#,
      user_id,
      blog_id
    )
    .fetch_one(conn)
    .await?;

    Ok(record.exists)
  }

  pub async fn create_new(
    conn: &mut PgConnection,
    user_id: i64,
    blog_id: i64,
    is_admin: Option<bool>,
  ) -> Result<()> {
    sqlx::query!(
      r#"
INSERT INTO user_blogs (user_id, blog_id, is_admin)
VALUES ($1, $2, $3)
      "#,
      user_id,
      blog_id,
      is_admin
    )
    .execute(conn)
    .await?;

    Ok(())
  }
}
