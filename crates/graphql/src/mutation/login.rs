use async_graphql::{Context, ErrorExtensions, Result};
use sqlx::PgPool;

use tumblepub_events::models::user::User;
use tumblepub_utils::errors::TumblepubError;

use crate::models::AuthPayload;

pub async fn login(ctx: &Context<'_>, email: String, password: String) -> Result<AuthPayload> {
  let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();
  let user = User::verify(&mut pool, email, password)
    .await
    .map_err(|_| TumblepubError::Unauthorized.extend())?
    .ok_or_else(|| TumblepubError::NotFound.extend())?;

  Ok(AuthPayload::new(user))
}
