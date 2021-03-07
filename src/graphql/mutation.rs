use juniper::graphql_object;

use crate::errors::ServiceResult;

use super::{models, Context};

pub struct Mutation;
// #[graphql_object(context = Context)]
impl Mutation {
  // pub fn login(context: &Context) -> ServiceResult<models::user::User> {}
  // pub fn register(context: &Context) -> ServiceResult<models::user::User> {}
}
