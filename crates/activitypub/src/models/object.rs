use chrono::NaiveDateTime;
use serde::{Deserialize, Serialize};
use serde_json::Value;

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "PascalCase")]
pub enum ObjectKind {
  Article,
  Audio,
  Document,
  Event,
  Image,
  Note,
  Page,
  Place,
  Profile,
  Relationship,
  Tombstone,
  Video,
}

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct Object {
  #[serde(rename = "@context")]
  pub context: Value,
  #[serde(rename = "type")]
  pub kind: ObjectKind,

  pub id: String,
  pub attributed_to: String,
  pub content: String,

  pub published: Option<NaiveDateTime>,
  pub to: Vec<String>,
  pub cc: Vec<String>,
}
