use serde::{Deserialize, Serialize};

#[derive(Debug, Deserialize, Serialize, Clone)]
pub struct SingleUserOptions {
  pub username: String,
  pub email: String,
  pub password: String,
}
