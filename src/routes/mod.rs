use actix_files as fs;

mod blog;
pub mod well_known;

pub async fn index() -> std::io::Result<fs::NamedFile> {
  Ok(fs::NamedFile::open("./build/index.html")?)
}
