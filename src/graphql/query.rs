use juniper::graphql_object;

use crate::{
  database::models::Blog,
  errors::{ServiceError, ServiceResult},
};

use super::{models, Context};

pub struct Query;
#[graphql_object(context = Context)]
impl Query {
  pub fn blog(context: &Context, name: String) -> ServiceResult<models::blog::Blog> {
    let blog = Blog::get_by_name(&context.db_pool, name)?;
    if let Some(blog) = blog {
      Ok(models::blog::Blog::from(blog))
    } else {
      Err(ServiceError::BadRequest("Invalid blog name".to_string()))
    }
  }
}
