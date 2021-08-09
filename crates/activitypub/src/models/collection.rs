use serde::{Deserialize, Serialize};
use serde_json::Value;

use super::{activity::Activity, object::Object};

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "PascalCase")]
pub enum CollectionKind {
  Collection,
  CollectionPage,
  OrderedCollection,
  OrderedCollectionPage,
}

#[derive(Deserialize, Serialize)]
#[serde(untagged)]
pub enum CollectionItem {
  Uri(String),
  Object(Object),
  Activity(Activity),
}

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct Collection {
  #[serde(rename = "@context")]
  pub context: Value,
  #[serde(rename = "type")]
  pub kind: CollectionKind,

  pub id: String,
  pub total_items: i64,
  #[serde(skip_serializing_if = "Vec::is_empty")]
  pub items: Vec<CollectionItem>,
  #[serde(skip_serializing_if = "Vec::is_empty")]
  pub ordered_items: Vec<CollectionItem>,

  #[serde(skip_serializing_if = "Option::is_none")]
  pub first: Option<String>,
}
