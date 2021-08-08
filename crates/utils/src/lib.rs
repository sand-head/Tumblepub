use once_cell::sync::Lazy;
use reqwest::Client;

pub mod crypto;
pub mod errors;
pub mod jwt;
pub mod markdown;
pub mod options;

pub static HTTP_CLIENT: Lazy<Client> = Lazy::new(|| Client::builder().build().unwrap());

pub fn description_to_html(desc: String) -> String {
  format!("<p>{}</p>", desc.replace('\n', "<br />"))
}
