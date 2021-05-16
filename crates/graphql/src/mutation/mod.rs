use async_graphql::{Context, ErrorExtensions, Object, Result};
use sqlx::PgPool;

use tumblepub_db::models::user::User;
use tumblepub_utils::{errors::TumblepubError, jwt::Token};

use self::{login::login, register::register};
use super::models::user::UserAuthPayload;
use crate::models::posts::{Post, TextPost};

mod login;
mod register;

pub struct Mutation;
#[Object]
impl Mutation {
  pub async fn login(
    &self,
    context: &Context<'_>,
    email: String,
    password: String,
  ) -> Result<UserAuthPayload> {
    login(context, email, password).await
  }

  pub async fn register(
    &self,
    context: &Context<'_>,
    email: String,
    password: String,
    name: String,
  ) -> Result<UserAuthPayload> {
    register(context, email, password, name).await
  }

  // todo: add parameters for post creation
  // there are no "input unions" (yet?) in GraphQL, so we'll have to find an alternative
  // worst case: we accept JSON encoded as a string :/
  pub async fn create_post(&self, ctx: &Context<'_>) -> Result<Post> {
    let token = ctx.data::<Token>()?;
    let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();
    let _user = User::get_by_token(&mut pool, token)
      .await
      .map_err(|_| TumblepubError::Unauthorized.extend())?
      .ok_or_else(|| TumblepubError::Unauthorized.extend())?;

    // todo: create the post in the database
    Ok(Post::Text(TextPost {
      content: "# Hello world!".to_string(),
      html_content: "<h1>Hello world!</h1>".to_string(),
    }))
  }
}
