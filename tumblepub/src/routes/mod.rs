use serde::Deserialize;

pub mod activitypub;
pub mod assets;
pub mod blog;
pub mod graphql;
pub mod well_known;

#[derive(Deserialize)]
pub struct BlogPath {
  pub blog: String,
}
