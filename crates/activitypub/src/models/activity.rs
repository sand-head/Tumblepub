use activitystreams::object::{ApObject, Object};
use serde::{Deserialize, Serialize};

#[derive(Deserialize, Serialize)]
#[serde(tag = "type")]
pub enum ActivityKind {
  Create { object: ApObject<Object<String>> },
  Announce,
  Follow,
}
#[derive(Deserialize, Serialize)]
pub struct Activity {
  pub actor: String,
  #[serde(flatten)]
  pub kind: ActivityKind,
}
