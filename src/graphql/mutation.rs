use juniper::graphql_object;

use crate::{
  database::models::{
    blog::{Blog, InsertableBlog},
    user::{InsertableUser, User},
  },
  errors::{ServiceError, ServiceResult},
};

use super::{
  models::{self, user::UserAuthPayload},
  Context,
};

pub struct Mutation;
#[graphql_object(context = Context)]
impl Mutation {
  pub async fn login(
    context: &Context,
    email: String,
    password: String,
  ) -> ServiceResult<models::user::UserAuthPayload> {
    let mut pool = context.db_pool.acquire().await.unwrap();
    let user = User::verify(&mut pool, email, password).await?;
    if let None = user {
      return Err(ServiceError::BadRequest("user does not exist".to_string()));
    }

    let user = user.unwrap();
    let blog = user.primary_blog(&mut pool).await?;

    Ok(UserAuthPayload::new(super::models::user::User::from((
      user, blog,
    ))))
  }

  pub async fn register(
    context: &Context,
    email: String,
    password: String,
    name: String,
  ) -> ServiceResult<models::user::UserAuthPayload> {
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

    // step 3: return auth payload with JWT
    let user = result.unwrap();
    Ok(UserAuthPayload::new(super::models::user::User::from((
      user,
      primary_blog,
    ))))
  }
}
