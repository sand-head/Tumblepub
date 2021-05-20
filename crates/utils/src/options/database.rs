use percent_encoding::{utf8_percent_encode, NON_ALPHANUMERIC};
use serde::{Deserialize, Serialize};

#[derive(Debug, Deserialize, Serialize, Clone)]
pub struct DatabaseOptions {
  pub db_name: String,
  pub hostname: String,
  pub port: i32,
  pub username: String,
  pub password: String,
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
      utf8_percent_encode(&database.password, NON_ALPHANUMERIC).to_string(),
      database.hostname,
      database.port,
      db_name.unwrap_or(database.db_name,)
    )
  }
}
