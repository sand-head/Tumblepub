use chrono::NaiveDateTime;
use serde::{Deserialize, Serialize};
use serde_json::Value;

use super::{actor::Actor, object::Object};

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "PascalCase")]
pub enum ActivityKind {
  Create,
  Announce,
  Follow,
  Accept,
  Update,
}

#[derive(Deserialize, Serialize)]
#[serde(untagged)]
pub enum ActivityObject {
  Uri(String),
  Actor(Actor),
  Object(Object),
  Activity(Box<Activity>),
}

#[derive(Deserialize, Serialize)]
pub struct Activity {
  #[serde(rename = "@context")]
  pub context: Value,
  #[serde(rename = "type")]
  pub kind: ActivityKind,

  pub actor: String,
  pub object: ActivityObject,

  pub published: Option<NaiveDateTime>,
  pub to: Vec<String>,
  pub cc: Vec<String>,
}
