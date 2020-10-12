use actix_files as fs;
use handlebars::Handlebars;

use crate::database;

mod blog;
pub mod well_known;

pub struct AppState<'hbs> {
  pub hbs: Handlebars<'hbs>,
  pub pool: database::DbPool,
}

pub async fn index() -> std::io::Result<fs::NamedFile> {
  Ok(fs::NamedFile::open("./build/index.html")?)
}
