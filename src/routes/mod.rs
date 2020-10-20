use std::sync::Arc;

use actix_files as fs;
use handlebars::Handlebars;

use crate::{database, graphql::Schema};

mod blog;
pub mod graphql;
pub mod well_known;

pub struct AppState<'hbs> {
  pub hbs: Handlebars<'hbs>,
  pub pool: database::DbPool,
  pub schema: Arc<Schema>
}

pub async fn index() -> std::io::Result<fs::NamedFile> {
  Ok(fs::NamedFile::open("./build/index.html")?)
}
