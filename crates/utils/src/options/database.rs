use serde::{Deserialize, Serialize};

#[derive(Debug, Deserialize, Serialize, Clone)]
pub struct DatabaseOptions {
  db_name: String,
  hostname: String,
  port: i32,
  username: String,
  password: String,
}

impl Default for DatabaseOptions {
  fn default() -> Self {
    Self {
      db_name: String::from("tumblepub"),
      hostname: String::from("localhost"),
      port: 5432,
      username: String::from("postgres"),
      password: String::from("mysecretpassword"),
    }
  }
}

impl DatabaseOptions {
  pub fn database_url(&self, db_name: Option<String>) -> String {
    let database = self.to_owned();

    format!(
      "postgres://{}:{}@{}:{}/{}",
      database.username,
      database.password,
      database.hostname,
      database.port,
      db_name.unwrap_or(database.db_name,)
    )
  }

  pub fn db_name(&self) -> String {
    self.db_name.to_owned()
  }
}
