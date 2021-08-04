use async_graphql::SimpleObject;
use tumblepub_db::models::user::User;
use tumblepub_utils::jwt::{Token, UserClaims};

use self::blog::Blog;

pub mod blog;
pub mod posts;

#[derive(Debug, Clone, SimpleObject)]
pub struct AuthPayload {
  pub token: String,
}
impl AuthPayload {
  pub fn new(user: User) -> Self {
    let claims = UserClaims { sub: user.id };
    Self {
      token: Token::new(claims).generate().unwrap(),
    }
  }
}

#[derive(Debug, Clone, SimpleObject)]
pub struct CurrentUser {
  pub blogs: Vec<Blog>,
}
