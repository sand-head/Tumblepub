pub mod crypto;
pub mod errors;
pub mod jwt;
pub mod markdown;
pub mod options;

pub fn description_to_html(desc: String) -> String {
  format!("<p>{}</p>", desc.replace('\n', "<br />"))
}
