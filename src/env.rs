use dotenv::{dotenv, from_filename};

pub fn load_dotenv() {
  let env = if cfg!(test) {
    "test"
  } else if cfg!(debug_assertions) {
    "development"
  } else {
    "production"
  };

  from_filename(format!(".env.{}.local", env)).ok();
  from_filename(".env.local").ok();
  from_filename(format!(".env.{}", env)).ok();
  dotenv().ok();
}