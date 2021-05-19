use async_graphql::{ComplexObject, Context, Result, SimpleObject};
use chrono::{DateTime, Utc};
use sqlx::PgPool;

use tumblepub_db::models::{self as db_models, blog::Blog as DbBlog};
use tumblepub_utils::jwt::{Token, UserClaims};

use super::blog::Blog;

#[derive(Debug, Clone, SimpleObject)]
#[graphql(complex)]
/// Information about a user
pub struct User {
  #[graphql(skip)]
  pub id: i64,
  #[graphql(skip)]
  pub primary_blog: i64,
  /// The user's username, based on the username of their primary blog
  pub name: String,
  /// The date and time that the user first joined
  pub joined_at: DateTime<Utc>,
}

#[ComplexObject]
impl User {
  async fn primary_blog(&self, ctx: &Context<'_>) -> Result<Blog> {
    let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();
    let blog = DbBlog::get_by_id(&mut pool, self.primary_blog)
      .await?
      .expect("user had no primary blog");
    Ok(blog.into())
  }
}

impl From<(db_models::user::User, db_models::blog::Blog)> for User {
  fn from((user, blog): (db_models::user::User, DbBlog)) -> Self {
    Self {
      id: user.id,
      primary_blog: user.primary_blog,
      name: blog.name,
      joined_at: user.created_at,
    }
  }
}

#[derive(Debug, Clone, SimpleObject)]
pub struct UserAuthPayload {
  pub user: User,
  pub token: String,
}
impl UserAuthPayload {
  pub fn new(user: User) -> Self {
    let claims = UserClaims { sub: user.id };
    Self {
      user,
      token: Token::new(claims).generate().unwrap(),
    }
  }
}
