use actix_files as fs;

pub mod blog;
pub mod graphql;
pub mod well_known;

pub async fn index() -> std::io::Result<fs::NamedFile> {
  Ok(fs::NamedFile::open("./build/index.html")?)
}
