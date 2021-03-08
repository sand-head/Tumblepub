/// Represents a many-to-many relationship between users and blogs.
#[derive(sqlx::FromRow, Debug)]
pub struct UserBlogs {
  pub user_id: i64,
  pub blog_id: i64,
  pub is_admin: bool,
}
