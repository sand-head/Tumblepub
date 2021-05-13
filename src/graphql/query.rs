use juniper::graphql_object;
use tumblepub_db::models::blog::Blog;

use crate::errors::{ServiceError, ServiceResult};

use super::{models, Context};

pub struct Query;
#[graphql_object(context = Context)]
impl Query {
  pub fn apiVersion() -> String {
    "0.1".to_string()
  }

  #[graphql(arguments(name(description = "The name of the blog")))]
  pub async fn blog(
    context: &Context,
    name: String,
    domain: Option<String>,
  ) -> ServiceResult<Option<models::blog::Blog>> {
    let mut pool = context.db_pool.acquire().await.unwrap();
    match Blog::find(&mut pool, (name, domain)).await {
      Ok(b) => Ok(b.map(|b| b.into())),
      Err(e) => Err(ServiceError::BadRequest(e.to_string())),
    }
  }
}
