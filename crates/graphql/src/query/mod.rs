use async_graphql::{Context, ErrorExtensions, Object, Result};
use sqlx::PgPool;

use tumblepub_db::models::{blog::Blog as DbBlog, user::User as DbUser};
use tumblepub_utils::{errors::TumblepubError, jwt::Token};

use super::models::{blog::Blog, user::User};

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
  ) -> Result<Option<Blog>> {
    let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();
    match DbBlog::find(&mut pool, (name, domain)).await {
      Ok(b) => Ok(b.map(|b| b.into())),
      Err(_) => Err(TumblepubError::NotFound.extend()),
    }
  }

  /// Gets the currently authenticated user.
  pub async fn current_user(&self, ctx: &Context<'_>) -> Result<User> {
    let token = ctx.data::<Token>()?;
    let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();

    let user = DbUser::get_by_token(&mut pool, token)
      .await
      .map_err(|_| TumblepubError::Unauthorized.extend())?
      .ok_or_else(|| TumblepubError::Unauthorized.extend())?;
    let blog = user.primary_blog(&mut pool).await?;

    Ok(User::from((user, blog)))
  }
}
