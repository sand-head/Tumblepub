use async_graphql::SimpleObject;
use chrono::NaiveDateTime;
use jsonwebtoken::{encode, EncodingKey, Header};
use serde::Serialize;

use tumblepub_db::models as db_models;

#[derive(Debug, Clone, SimpleObject)]
/// Information about a user
pub struct User {
  /// The user's username, based on the username of their primary blog
  pub name: String,
  /// The date and time that the user first joined
  pub joined_at: NaiveDateTime,
}
impl From<(db_models::user::User, db_models::blog::Blog)> for User {
  fn from((user, blog): (db_models::user::User, db_models::blog::Blog)) -> Self {
    Self {
      name: blog.name,
      joined_at: user.created_at,
    }
  }
}

#[derive(Serialize)]
struct UserClaims {}

#[derive(Debug, Clone, SimpleObject)]
pub struct UserAuthPayload {
  pub user: User,
  pub token: String,
}
impl UserAuthPayload {
  pub fn new(user: User) -> Self {
    let claims = UserClaims {};
    Self {
      user,
      token: encode(
        &Header::default(),
        &claims,
        // todo: do not hardcode the secret
        &EncodingKey::from_secret("big boy secret time".as_bytes()),
      )
      .unwrap(),
    }
  }
}
