use diesel::prelude::*;

use crate::{
  database::connect,
  database::schema::{user_blogs, users},
  errors::ServiceResult,
};
use crate::{database::DbPool, errors::ServiceError};

// users are like a "secret" thing in context of the site
// they exist, and are used for authentication
// but the user-facing "user" type is blogs
// only a user themselves can see their own data
#[derive(Identifiable, Queryable)]
#[table_name = "users"]
pub struct User {
  pub id: i64,
  pub email: String,
  pub primary_blog: i64,
  pub hash: Vec<u8>,
  pub salt: String,
}

impl User {
  pub fn get_by_id(pool: &DbPool, user_id: i64) -> ServiceResult<Option<User>> {
    use crate::database::schema::users::dsl::{id, users};

    users
      .filter(id.eq(user_id))
      .first::<User>(&connect(&pool)?)
      .optional()
      .map_err(|_| ServiceError::Unauthorized)
  }
}

/// Represents a many-to-many relationship between users and blogs.
#[derive(Identifiable, Queryable)]
#[table_name = "user_blogs"]
#[primary_key(user_id, blog_id)]
pub struct UserBlogs {
  pub user_id: i64,
  pub blog_id: i64,
  pub is_admin: bool,
}
