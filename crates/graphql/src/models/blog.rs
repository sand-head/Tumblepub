use async_graphql::{ComplexObject, Context, Result, SimpleObject};
use sqlx::PgPool;

use tumblepub_db::models::blog::Blog as DbBlog;

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
  /// Paginates through a blog's posts.
  async fn posts(
    &self,
    ctx: &Context<'_>,
    #[graphql(default = 25)] limit: i32,
    #[graphql(default)] offset: i32,
  ) -> Result<Vec<Post>> {
    let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();
    let blog = DbBlog::get_by_id(&mut pool, self.id).await?.unwrap();
    let posts: Vec<Post> = blog
      .posts(&mut pool, Some(limit), Some(offset))
      .await?
      .iter()
      .map(Post::from)
      .collect();

    Ok(posts)
  }
}

impl From<DbBlog> for Blog {
  fn from(blog: DbBlog) -> Self {
    Self {
      id: blog.id,
      name: blog.name,
      domain: blog.domain,
      title: blog.title,
      description: blog.description,
    }
  }
}
