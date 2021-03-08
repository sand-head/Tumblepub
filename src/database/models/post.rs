use uuid::Uuid;

#[derive(sqlx::FromRow)]
pub struct Post {
  pub id: Uuid,
  pub blog_id: i64,
  // todo: determine how post content is stored
}
