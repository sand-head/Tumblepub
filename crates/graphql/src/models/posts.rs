use async_graphql::{SimpleObject, Union};

#[derive(Debug, SimpleObject)]
pub struct TextPost {
  pub content: String,
  pub html_content: String,
}

#[derive(Debug, Union)]
pub enum Post {
  Text(TextPost),
}
