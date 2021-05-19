use chrono::NaiveDateTime;
use handlebars::{handlebars_helper, no_escape, Handlebars, TemplateError};
use percent_encoding::{utf8_percent_encode, NON_ALPHANUMERIC};
use serde::Serialize;

handlebars_helper!(url_encode: |v: str| utf8_percent_encode(v, NON_ALPHANUMERIC).to_string());

#[derive(Serialize)]
#[serde(rename_all = "PascalCase", tag = "Type")]
pub enum ThemePostContent {
  Markdown {
    #[serde(rename = "Content")]
    content: String,
  },
}

#[derive(Serialize)]
#[serde(rename_all = "PascalCase")]
pub struct ThemePost {
  pub created_at: NaiveDateTime,
  pub content: Vec<ThemePostContent>,
}

#[derive(Serialize)]
#[serde(rename_all = "PascalCase")]
pub struct ThemeVariables {
  pub title: String,
  pub description: Option<String>,
  pub posts: Vec<ThemePost>,

  pub previous_page: Option<String>,
  pub next_page: Option<String>,
}

pub fn create_handlebars() -> Result<Handlebars<'static>, TemplateError> {
  let mut hbs = Handlebars::new();
  let default_theme = include_str!("../themes/default.hbs");
  hbs.register_template_string("default", default_theme)?;
  hbs.register_helper("url", Box::new(url_encode));
  hbs.register_escape_fn(no_escape);
  hbs.set_strict_mode(true);
  Ok(hbs)
}
