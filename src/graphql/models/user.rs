use chrono::NaiveDateTime;
use jsonwebtoken::{encode, EncodingKey, Header};
use juniper::GraphQLObject;
use serde::Serialize;

use crate::database;

#[derive(GraphQLObject)]
/// Information about a user
pub struct User {
  /// The user's username, based on the username of their primary blog
  pub name: String,
  /// The date and time that the user first joined
  pub joined_at: NaiveDateTime,
}
impl From<(database::models::user::User, database::models::blog::Blog)> for User {
  fn from((user, blog): (database::models::user::User, database::models::blog::Blog)) -> Self {
    Self {
      name: blog.name,
      joined_at: user.created_at,
    }
  }
}

#[derive(Serialize)]
struct UserClaims {}

#[derive(GraphQLObject)]
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
