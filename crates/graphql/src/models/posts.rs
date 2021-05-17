use std::convert::TryInto;

use async_graphql::{InputObject, SimpleObject, Union};

use chrono::NaiveDateTime;
use tumblepub_db::models::post::{Post as DbPost, PostContent as DbPostContent};
use tumblepub_utils::markdown::markdown_to_safe_html;

#[derive(Debug, SimpleObject)]
pub struct Post {
  pub content: Vec<PostContent>,
  pub created_at: NaiveDateTime,
}

#[derive(Debug, Union)]
pub enum PostContent {
  Markdown(MarkdownContent),
}

#[derive(Debug, SimpleObject)]
pub struct MarkdownContent {
  pub content: String,
  pub html_content: String,
}

impl From<DbPost> for Post {
  fn from(post: DbPost) -> Self {
    Post {
      created_at: post.created_at,
      content: post
        .content
        .iter()
        .map(|content| match content {
          DbPostContent::Markdown(markdown) => PostContent::Markdown(MarkdownContent {
            content: markdown.to_string(),
            html_content: markdown_to_safe_html(markdown),
          }),
        })
        .collect(),
    }
  }
}
impl From<&DbPost> for Post {
  fn from(post: &DbPost) -> Self {
    Post {
      created_at: post.created_at,
      content: post
        .content
        .iter()
        .map(|content| match content {
          DbPostContent::Markdown(markdown) => PostContent::Markdown(MarkdownContent {
            content: markdown.to_string(),
            html_content: markdown_to_safe_html(markdown),
          }),
        })
        .collect(),
    }
  }
}

#[derive(Debug, InputObject)]
pub struct PostInput {
  pub content: Vec<PostContentInput>,
}

#[derive(Debug, InputObject)]
/// The content of the post, made up of chunks. One and only one property may be filled.
pub struct PostContentInput {
  /// A chunk of Markdown.
  pub markdown: Option<String>,
}

impl TryInto<DbPostContent> for &PostContentInput {
  type Error = anyhow::Error;

  fn try_into(self) -> Result<DbPostContent, Self::Error> {
    // hey in rust 1.53 we finally get iterating over arrays
    // todo: do that for this instead of hard-coding all of this for markdown
    let chunk = self
      .markdown
      .as_ref()
      .ok_or_else(|| anyhow::anyhow!("no chunks were provided in input"))?;
    Ok(DbPostContent::Markdown(chunk.to_string()))
  }
}
