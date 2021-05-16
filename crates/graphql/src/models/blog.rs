use async_graphql::{ComplexObject, Context, SimpleObject};

use tumblepub_db::models as db_models;
use tumblepub_utils::errors::Result;

use super::posts::Post;

#[derive(Debug, Clone, SimpleObject)]
#[graphql(complex)]
/// Information about a blog
pub struct Blog {
  #[graphql(skip)]
  id: i64,
  /// The blog's username
  pub name: String,
  /// The blog's optional domain
  pub domain: Option<String>,
  /// The blog's optional title
  pub title: Option<String>,
  /// The blog's optional description
  pub description: Option<String>,
}

#[ComplexObject]
impl Blog {
  async fn posts(
    &self,
    ctx: &Context<'_>,
    #[graphql(default = 25)] limit: i32,
    #[graphql(default)] offset: i32,
  ) -> Result<Vec<Post>> {
    // todo: query database for posts
    Ok(vec![])
  }
}

impl From<db_models::blog::Blog> for Blog {
  fn from(blog: db_models::blog::Blog) -> Self {
    Self {
      id: blog.id,
      name: blog.name,
      domain: blog.domain,
      title: blog.title,
      description: blog.description,
    }
  }
}
