fn main() -> Result<(), std::io::Error> {
  npm_rs::NpmEnv::default()
    .with_env("NODE_ENV", "production")
    .set_path("../theme")
    .init()
    .install(None)
    .run("build")
    .exec()?;

  Ok(())
}
