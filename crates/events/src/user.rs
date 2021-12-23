/// Represents a user's data and settings.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct User {
  pub id: i64,
  pub email: String,
  pub hash: Vec<u8>,
  pub salt: String,
  pub created_at: DateTime<Utc>,
  pub updated_at: DateTime<Utc>,
  pub last_login_at: DateTime<Utc>,
}
