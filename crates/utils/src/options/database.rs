use merge::Merge;
use serde::{Deserialize, Serialize};

#[derive(Debug, Deserialize, Serialize, Clone, Merge)]
pub struct DatabaseOptions {
  pub(super) database: Option<String>,
  pub(super) hostname: Option<String>,
  pub(super) port: Option<i32>,
  pub(super) username: Option<String>,
  pub(super) password: Option<String>,
}

impl Default for DatabaseOptions {
  fn default() -> Self {
    Self {
      database: Some("tumblepub".to_string()),
      hostname: Some("localhost".to_string()),
      port: Some(5432),
      username: Some("postgres".to_string()),
      password: Some("mysecretpassword".to_string()),
    }
  }
}
