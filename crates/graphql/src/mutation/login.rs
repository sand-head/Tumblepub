use async_graphql::{Context, ErrorExtensions, Result};
use sqlx::PgPool;

use tumblepub_db::models::user::User;
use tumblepub_utils::errors::TumblepubError;

use crate::models::user::{User as GqlUser, UserAuthPayload};

pub async fn login(ctx: &Context<'_>, email: String, password: String) -> Result<UserAuthPayload> {
  let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();
  let user = User::verify(&mut pool, email, password)
    .await
    .expect("could not verify user");
  if let None = user {
    return Err(TumblepubError::NotFound.extend());
  }

  let user = user.unwrap();
  let blog = user.primary_blog(&mut pool).await.unwrap();

  Ok(UserAuthPayload::new(GqlUser::from((user, blog))))
}
