use activitystreams::object::{ApObject, Object};
use serde::{Deserialize, Serialize};
use serde_json::Value;

#[derive(Deserialize, Serialize)]
#[serde(tag = "type")]
pub enum ActivityKind {
  Create { object: ApObject<Object<String>> },
  Announce { object: ApObject<Object<String>> },
  Follow { object: String },
  Accept,
}
#[derive(Deserialize, Serialize)]
pub struct Activity {
  #[serde(rename = "@context")]
  pub context: Value,
  #[serde(flatten)]
  pub kind: ActivityKind,

  pub actor: String,
  pub to: Vec<String>,
  pub cc: Vec<String>,
}
