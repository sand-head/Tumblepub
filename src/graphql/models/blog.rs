use juniper::GraphQLObject;

use tumblepub_db::models as db_models;

#[derive(GraphQLObject)]
/// Information about a blog
pub struct Blog {
  /// The blog's username
  name: String,
  /// The blog's optional domain
  domain: Option<String>,
  /// The blog's optional title
  title: Option<String>,
  /// The blog's optional description
  description: Option<String>,
}

impl From<db_models::blog::Blog> for Blog {
  fn from(blog: db_models::blog::Blog) -> Self {
    Self {
      name: blog.name,
      domain: blog.domain,
      title: blog.title,
      description: blog.description,
    }
  }
}
