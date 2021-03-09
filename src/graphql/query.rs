use juniper::graphql_object;

use crate::{database::models::blog::Blog, errors::ServiceResult};

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
    match Blog::find(&context.db_pool, (name, domain)).await {
      Ok(b) => Ok(b.map(|b| b.into())),
      Err(e) => Err(e),
    }
  }
}
