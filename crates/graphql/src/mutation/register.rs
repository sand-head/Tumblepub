use async_graphql::{ErrorExtensions, Result};

use sqlx::PgPool;
use tumblepub_events::models::{
  blog::{Blog, NewBlog},
  user::{NewUser, User},
};
use tumblepub_utils::{crypto::KeyPair, errors::TumblepubError};

use crate::models::AuthPayload;

pub async fn register(
  pool: &PgPool,
  email: String,
  password: String,
  name: String,
) -> Result<AuthPayload> {
  let mut txn = pool.begin().await?;
  // step 1: create a new user
  let user = User::create(&mut *txn, NewUser { email, password })
    .await
    .map_err(|e| TumblepubError::InternalServerError(e).extend())?;

  // step 2: create a primary blog for the new user using the provided name
  let keypair = KeyPair::generate().map_err(|e| TumblepubError::InternalServerError(e).extend())?;
  Blog::create_new(
    &mut *txn,
    NewBlog {
      user_id: Some(user.id),
      uri: None,
      name,
      domain: None,
      is_primary: true,
      is_public: true,
      title: None,
      description: None,
      private_key: Some(keypair.private_key),
      public_key: keypair.public_key,
    },
  )
  .await
  .map_err(|e| TumblepubError::InternalServerError(e).extend())?;

  // step 3: commit transaction and return auth payload with JWT
  txn.commit().await?;
  Ok(AuthPayload::new(user))
}
