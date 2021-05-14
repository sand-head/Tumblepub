use async_graphql::{Context, ErrorExtensions, Object, Result};

use sqlx::PgPool;
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::errors::TumblepubError;

use super::models;

pub struct Query;
#[Object]
impl Query {
  pub async fn api_version(&self) -> String {
    "0.1".to_string()
  }

  pub async fn blog(
    &self,
    ctx: &Context<'_>,
    name: String,
    domain: Option<String>,
  ) -> Result<Option<models::blog::Blog>> {
    let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();
    match Blog::find(&mut pool, (name, domain)).await {
      Ok(b) => Ok(b.map(|b| b.into())),
      Err(_) => Err(TumblepubError::NotFound.extend()),
    }
  }
}
