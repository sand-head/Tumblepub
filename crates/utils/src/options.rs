use std::{fs::read_to_string, sync::RwLock};

use anyhow::Result;
use once_cell::sync::Lazy;
use serde::{Deserialize, Serialize};

static OPTIONS_LOCATION: &str = "./options.yaml";
static OPTIONS: Lazy<RwLock<Options>> =
  Lazy::new(|| RwLock::new(Options::load().expect("failed to read options.yaml")));

#[derive(Debug, Deserialize, Serialize, Clone)]
pub struct Options {
  pub single_user_mode: bool,
}
impl Default for Options {
  fn default() -> Self {
    Self {
      single_user_mode: true,
    }
  }
}

impl Options {
  pub fn load() -> Result<Self> {
    // todo: load options from env
    Ok(serde_yaml::from_str(&read_to_string(OPTIONS_LOCATION)?)?)
  }

  pub fn get() -> Self {
    OPTIONS.read().unwrap().to_owned()
  }
}
