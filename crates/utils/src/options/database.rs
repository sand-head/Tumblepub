use merge::Merge;
use serde::{Deserialize, Serialize};

#[derive(Debug, Deserialize, Serialize, Clone, Merge)]
pub struct DatabaseOptions {
  pub(super) db_name: Option<String>,
  pub(super) hostname: Option<String>,
  pub(super) port: Option<i32>,
  pub(super) username: Option<String>,
  pub(super) password: Option<String>,
}

impl Default for DatabaseOptions {
  fn default() -> Self {
    Self {
      db_name: Some("tumblepub".to_string()),
      hostname: Some("localhost".to_string()),
      port: Some(5432),
      username: Some("postgres".to_string()),
      password: Some("mysecretpassword".to_string()),
    }
  }
}

impl DatabaseOptions {
  pub fn database_url(&self, db_name: Option<String>) -> String {
    let database = self.to_owned();

    format!(
      "postgres://{}:{}@{}:{}/{}",
      database
        .username
        .expect("could not get database username from options"),
      database
        .password
        .expect("could not get database password from options"),
      database
        .hostname
        .expect("could not get database hostname from options"),
      database
        .port
        .expect("could not get database port from options"),
      db_name.unwrap_or(
        database
          .db_name
          .expect("could not get database name from options"),
      )
    )
  }

  pub fn db_name(&self) -> String {
    self
      .db_name
      .to_owned()
      .expect("could not retrieve database name from options")
  }
}
