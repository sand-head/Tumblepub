use async_graphql::{Context, ErrorExtensions, Result};
use sqlx::PgPool;

use tumblepub_db::models::user::User;
use tumblepub_utils::errors::TumblepubError;

use crate::models::user::{User as GqlUser, UserAuthPayload};

pub async fn login(ctx: &Context<'_>, email: String, password: String) -> Result<UserAuthPayload> {
  let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();
  let user = User::verify(&mut pool, email, password)
    .await
    .map_err(|_| TumblepubError::Unauthorized.extend())?
    .ok_or_else(|| TumblepubError::NotFound.extend())?;

  let blog = user.primary_blog(&mut pool).await.unwrap();
  Ok(UserAuthPayload::new(GqlUser::from((user, blog))))
}
