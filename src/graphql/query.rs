use juniper::graphql_object;

use crate::{
  database::queries,
  errors::{ServiceError, ServiceResult},
};

use super::{models, Context};

pub struct Query;
#[graphql_object(context = Context)]
impl Query {
  pub async fn blog(context: &Context, name: String) -> ServiceResult<models::blog::Blog> {
    let blog = queries::blog::get_by_name(&context.db_pool, name).await?;
    if let Some(blog) = blog {
      Ok(models::blog::Blog::from(blog))
    } else {
      Err(ServiceError::BadRequest("Invalid blog name".to_string()))
    }
  }
}
