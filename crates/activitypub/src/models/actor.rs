use chrono::NaiveDateTime;
use serde::{Deserialize, Serialize};
use serde_json::Value;

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "PascalCase")]
pub enum ActorKind {
  Person,
}

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct PublicKey {
  pub id: String,
  pub owner: String,
  pub public_key_pem: String,
}

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct Actor {
  #[serde(rename = "@context")]
  pub context: Value,
  #[serde(rename = "type")]
  pub kind: ActorKind,

  pub id: String,
  pub name: String,
  pub preferred_username: String,
  pub summary: Option<String>,
  pub published: NaiveDateTime,

  pub inbox: String,
  pub outbox: String,
  pub followers: Option<String>,
  pub following: Option<String>,

  pub public_key: PublicKey,
}
