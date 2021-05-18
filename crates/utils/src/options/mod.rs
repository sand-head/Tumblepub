use std::sync::RwLock;

use anyhow::Result;
use config::{Config, ConfigError, Environment, File, FileFormat};
use once_cell::sync::Lazy;
use serde::{Deserialize, Serialize};

use self::database::DatabaseOptions;

mod database;

static OPTIONS_LOCATION: &str = "./options.yml";
static OPTIONS: Lazy<RwLock<Options>> =
  Lazy::new(|| RwLock::new(Options::initialize().expect("failed to initialize options")));

#[derive(Debug, Deserialize, Serialize, Clone)]
pub struct Options {
  single_user_mode: bool,
  local_domain: String,
  database: DatabaseOptions,
}

impl Default for Options {
  fn default() -> Self {
    Self {
      single_user_mode: true,
      local_domain: String::from("example.com"),
      database: DatabaseOptions::default(),
    }
  }
}

impl Options {
  /// Initialize a new `Options` using environment variables, the `options.yml` file, and `Options::default()`.
  pub fn initialize() -> Result<Self, ConfigError> {
    let mut config = Config::default();

    // options priority:
    // the `config` crate merges on top of anything already there
    // so we go in order of lowest priority first
    // 3. default values
    config.merge(File::from_str(
      &serde_yaml::to_string(&Options::default()).unwrap(),
      FileFormat::Yaml,
    ))?;
    // 2. the options.yml file
    config.merge(File::with_name(OPTIONS_LOCATION).required(false))?;
    // 1. environment variables
    config.merge(Environment::with_prefix("TUMBLEPUB_"))?;

    config.try_into()
  }

  /// Get a cloned `Options`.
  pub fn get() -> Self {
    OPTIONS.read().unwrap().to_owned()
  }

  pub fn single_user_mode(&self) -> bool {
    self.single_user_mode
  }
  pub fn local_domain(&self) -> String {
    self.local_domain.to_owned()
  }
  pub fn database(&self) -> DatabaseOptions {
    self.database.to_owned()
  }
}
