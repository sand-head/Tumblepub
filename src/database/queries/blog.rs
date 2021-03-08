use sqlx::PgPool;

use crate::{database::models::blog::Blog, errors::ServiceResult};

pub async fn get_by_name(pool: &PgPool, name: String) -> ServiceResult<Option<Blog>> {
  Ok(
    sqlx::query_as::<_, Blog>("SELECT * FROM blogs WHERE name = ?")
      .bind(name)
      .fetch_optional(pool)
      .await?,
  )
}
