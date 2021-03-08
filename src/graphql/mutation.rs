use juniper::graphql_object;

use crate::{
  database::models::user::{InsertableUser, User},
  errors::ServiceResult,
};

use super::{models, Context};

pub struct Mutation;
#[graphql_object(context = Context)]
impl Mutation {
  // pub fn login(context: &Context) -> ServiceResult<models::user::User> {}
  pub async fn register(
    context: &Context,
    email: String,
    password: String,
    name: String,
  ) -> ServiceResult<models::user::User> {
    let (user, blog) = User::create(
      &context.db_pool,
      InsertableUser {
        email,
        password,
        name,
      },
    )
    .await?;

    Ok(models::user::User {
      name: blog.name,
      joined_at: user.created_at,
    })
  }
}
