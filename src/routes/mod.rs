use actix_files as fs;

mod blog;

pub async fn index() -> std::io::Result<fs::NamedFile> {
  Ok(fs::NamedFile::open("./build/index.html")?)
}