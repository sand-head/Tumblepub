use handlebars::{handlebars_helper, Handlebars};
use percent_encoding::{utf8_percent_encode, NON_ALPHANUMERIC};
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
#[serde(rename_all = "PascalCase")]
pub struct ThemeVariables {
  pub title: String,
  pub description: Option<String>,
  pub content: String,

  pub previous_page: Option<String>,
  pub next_page: Option<String>,
}

handlebars_helper!(url_encode: |v: str| utf8_percent_encode(v, NON_ALPHANUMERIC).to_string());

pub fn create_handlebars() -> Handlebars<'static> {
  let mut hbs = Handlebars::new();
  hbs
    .register_template_file("index", "./themes/default.hbs")
    .expect("./themes/default.hbs not found");
  hbs.register_helper("url", Box::new(url_encode));
  hbs
}
