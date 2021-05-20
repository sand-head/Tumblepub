use async_graphql::{ErrorExtensions, Result};

use sqlx::PgPool;
use tumblepub_ap::crypto::KeyPair;
use tumblepub_db::models::{
  blog::{Blog, NewBlog},
  user::{NewUser, User},
};
use tumblepub_utils::errors::TumblepubError;

use crate::models::user::{User as GqlUser, UserAuthPayload};

pub async fn register(
  pool: &PgPool,
  email: String,
  password: String,
  name: String,
) -> Result<UserAuthPayload> {
  let mut txn = pool.begin().await?;
  // step 1: create a primary blog for the new user
  let keypair = KeyPair::generate().map_err(|e| TumblepubError::InternalServerError(e).extend())?;
  let primary_blog = Blog::create_new(
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
  .await
  .map_err(|e| TumblepubError::InternalServerError(e).extend())?;

  // step 2: create a new user using the created primary blog ID
  let user = User::create(
    &mut *txn,
    NewUser {
      email,
      password,
      primary_blog: primary_blog.id,
    },
  )
  .await
  .map_err(|e| TumblepubError::InternalServerError(e).extend())?;

  // step 3: commit transaction and return auth payload with JWT
  txn.commit().await?;
  Ok(UserAuthPayload::new(GqlUser::from((user, primary_blog))))
}
