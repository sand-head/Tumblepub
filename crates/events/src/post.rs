#[derive(Debug)]
pub struct Post {
  #[serde(skip_serializing)]
  pub id: Uuid,
  #[serde(skip_serializing)]
  pub blog_id: i64,
  pub content: Json<Vec<PostContent>>,
  pub created_at: DateTime<Utc>,
  pub updated_at: DateTime<Utc>,
}
