use std::{fs::read_to_string, path::Path, sync::RwLock};

use anyhow::Result;
use merge::Merge;
use once_cell::sync::Lazy;
use serde::{Deserialize, Serialize};

use self::database::DatabaseOptions;

mod database;

static OPTIONS_LOCATION: &str = "./options.yaml";
static OPTIONS: Lazy<RwLock<Options>> =
  Lazy::new(|| RwLock::new(Options::initialize().expect("failed to initialize options")));

#[derive(Debug, Deserialize, Serialize, Clone, Merge)]
pub struct Options {
  single_user_mode: Option<bool>,
  local_domain: Option<String>,
  database: Option<DatabaseOptions>,
}

impl Default for Options {
  fn default() -> Self {
    Self {
      single_user_mode: Some(true),
      local_domain: None,
      database: Some(DatabaseOptions::default()),
    }
  }
}

impl Options {
  /// Initialize a new `Options` using environment variables, the `options.yaml` file, and `Options::default()`.
  pub fn initialize() -> Result<Self> {
    let options_path = Path::new(OPTIONS_LOCATION);

    // options priority:
    // 1. environment variables
    let mut options: Options = envy::prefixed("TUMBLEPUB_").from_env()?;
    // 2. the options.yaml file
    if options_path.is_file() {
      options.merge(serde_yaml::from_str(&read_to_string(OPTIONS_LOCATION)?)?);
    };
    // 3. default values
    options.merge(Options::default());

    Ok(options)
  }

  /// Get a cloned `Options`.
  pub fn get() -> Self {
    OPTIONS.read().unwrap().to_owned()
  }

  pub fn database_url(&self) -> Result<String> {
    let database = self
      .database
      .to_owned()
      .expect("could not get database options");

    Ok(format!(
      "postgres://{}:{}@{}:{}/{}",
      database.username.expect("could not get database username"),
      database.password.expect("could not get database password"),
      database.hostname.expect("could not get database hostname"),
      database.port.expect("could not get database port"),
      database.database.expect("could not get database name")
    ))
  }

  pub fn single_user_mode(&self) -> bool {
    self
      .single_user_mode
      .expect("could not determine if in single user mode")
  }
  pub fn local_domain(&self) -> String {
    self.local_domain.to_owned().unwrap()
  }
}
