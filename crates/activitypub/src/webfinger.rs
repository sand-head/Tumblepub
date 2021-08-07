use once_cell::sync::Lazy;
use regex::Regex;

pub static WEBFINGER_URI: Lazy<Regex> =
  Lazy::new(|| Regex::new("^acct:([a-z0-9_]*)@(.*)$").unwrap());

pub async fn webfinger(acct: String) {}
