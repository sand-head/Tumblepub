use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
#[serde(rename_all = "PascalCase")]
pub struct ThemeVariables {
  pub title: String,
  pub description: Option<String>,
  pub content: String,

  pub previous_page: Option<String>,
  pub next_page: Option<String>
}