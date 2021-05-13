use async_graphql::{Context, ErrorExtensions, Result};

use sqlx::PgPool;
use tumblepub_ap::crypto::KeyPair;
use tumblepub_db::models::{
  blog::{Blog, NewBlog},
  user::{InsertableUser, User},
};
use tumblepub_utils::errors::TumblepubError;

use crate::models::user::{User as GqlUser, UserAuthPayload};

pub async fn register(
  ctx: &Context<'_>,
  email: String,
  password: String,
  name: String,
) -> Result<UserAuthPayload> {
  let mut txn = ctx.data::<PgPool>()?.begin().await?;
  // step 1: create a primary blog for the new user
  let keypair = KeyPair::generate().expect("Could not generate public and private keys for user");
  let result = Blog::create_new(
    &mut *txn,
    NewBlog {
      uri: Option::<String>::None,
      name,
      domain: Option::<String>::None,
      is_public: true,
      title: Option::<String>::None,
      description: Option::<String>::None,
      private_key: keypair.private_key,
      public_key: keypair.public_key,
    },
  )
  .await;
  if let Err(e) = result {
    // not really sure if we *need* to manually rollback the transaction yet
    // better safe than sorry though!
    txn.rollback().await?;
    return Err(TumblepubError::InternalServerError(e).extend());
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
    return Err(TumblepubError::InternalServerError(e).extend());
  }

  // step 3: return auth payload with JWT
  let user = result.unwrap();
  Ok(UserAuthPayload::new(GqlUser::from((user, primary_blog))))
}
