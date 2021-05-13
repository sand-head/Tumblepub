use async_graphql::{Context, Object, Result};

use self::{login::login, register::register};
use super::models::user::UserAuthPayload;

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
}
