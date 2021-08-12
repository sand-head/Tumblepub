use std::process::{Command, Stdio};

fn main() -> anyhow::Result<()> {
  println!(r#"cargo:rerun-if-changed=build.rs"#);
  println!(r#"cargo:rerun-if-changed=theme"#);

  let dir = dunce::canonicalize("./theme")?;
  let command = Command::new("npm")
    .current_dir(dir.clone())
    .stdout(Stdio::piped())
    .arg("install")
    .spawn()?
    .wait_with_output()?;

  if command.status.success() {
    let command = Command::new("npm")
      .current_dir(dir)
      .stdout(Stdio::piped())
      .args(&["run", "build"])
      .spawn()?
      .wait_with_output()?;

    if !command.status.success() {
      panic!("{}", String::from_utf8(command.stdout)?);
    }
  } else {
    panic!("{}", String::from_utf8(command.stdout)?);
  }

  Ok(())
}
