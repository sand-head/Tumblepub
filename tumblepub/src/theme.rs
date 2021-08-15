use chrono::{DateTime, Local, Utc};
use handlebars::{handlebars_helper, no_escape, Handlebars, RenderError, TemplateError};
use percent_encoding::{utf8_percent_encode, NON_ALPHANUMERIC};
use serde::Serialize;
use timeago::Formatter;

handlebars_helper!(url_encode: |v: str| utf8_percent_encode(v, NON_ALPHANUMERIC).to_string());
handlebars_helper!(date_format: |iso_8601: str, format_str: str| {
  let date = DateTime::parse_from_rfc3339(iso_8601)
    .map_err(|_| RenderError::new("Could not parse first parameter as RFC3339 date."))?;
  format!("{}", date.format(format_str))
});
handlebars_helper!(time_ago: |iso_8601: str| {
  let date = DateTime::parse_from_rfc3339(iso_8601)
    .map_err(|_| RenderError::new("Could not parse first parameter as RFC3339 date."))?;
  let now = Utc::now();
  let formatter = Formatter::new();
  formatter.convert_chrono(date, now)
});

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
  pub created_at: DateTime<Local>,
  pub content: Vec<ThemePostContent>,
}

#[derive(Serialize)]
#[serde(rename_all = "PascalCase")]
pub struct ThemeVariables {
  pub title: String,
  pub description: Option<String>,
  pub avatar: String,
  pub posts: Vec<ThemePost>,

  pub previous_page: Option<String>,
  pub next_page: Option<String>,
}

pub fn create_handlebars() -> Result<Handlebars<'static>, TemplateError> {
  let mut hbs = Handlebars::new();
  let default_theme = include_str!("../../theme/dist/index.hbs");
  hbs.register_template_string("default", default_theme)?;
  hbs.register_helper("url", Box::new(url_encode));
  hbs.register_helper("formatDate", Box::new(date_format));
  hbs.register_helper("timeAgo", Box::new(time_ago));
  hbs.register_escape_fn(no_escape);
  hbs.set_strict_mode(true);
  Ok(hbs)
}
