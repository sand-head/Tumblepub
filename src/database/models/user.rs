use crate::database::schema::{user_blogs, users};

#[derive(Identifiable, Queryable)]
#[table_name = "users"]
pub struct User {
  pub id: i64,
  pub email: String,
  pub encrypted_password: String,
  pub primary_blog: i64,
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
