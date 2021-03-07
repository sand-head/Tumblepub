use diesel::prelude::*;

use crate::database::schema::user_blogs;

/// Represents a many-to-many relationship between users and blogs.
#[derive(Debug, Identifiable, Queryable)]
#[table_name = "user_blogs"]
#[primary_key(user_id, blog_id)]
pub struct UserBlogs {
  pub user_id: i64,
  pub blog_id: i64,
  pub is_admin: bool,
}
