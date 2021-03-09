use juniper::graphql_object;

use crate::{
  database::models::{
    blog::{Blog, InsertableBlog},
    user::{InsertableUser, User},
  },
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
    let mut txn = context.db_pool.begin().await?;
    // step 1: create a primary blog for the new user
    let result = Blog::create(
      &mut *txn,
      InsertableBlog {
        uri: Option::<String>::None,
        name,
        domain: Option::<String>::None,
        is_public: true,
        title: Option::<String>::None,
        description: Option::<String>::None,
      },
    )
    .await;
    if let Err(e) = result {
      // not really sure if we *need* to manually rollback the transaction yet
      // better safe than sorry though!
      txn.rollback().await?;
      return Err(e);
    }

    // step 2: create a new user using the created primary blog ID
    let primary_blog = result.unwrap();
    let result = User::create(
      &mut *txn,
      InsertableUser {
        email,
        password,
        primary_blog: primary_blog.id,
      },
    )
    .await;
    if let Err(e) = result {
      txn.rollback().await?;
      return Err(e);
    }

    let user = result.unwrap();
    Ok(models::user::User {
      name: primary_blog.name,
      joined_at: user.created_at,
    })
  }
}
