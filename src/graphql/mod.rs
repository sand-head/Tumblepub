use crate::errors::ServiceResult;
use crate::{
  database::{models::Blog, DbPool},
  errors::ServiceError,
};
use juniper::{graphql_object, EmptyMutation, EmptySubscription, RootNode};

pub mod models;

#[derive(Clone)]
pub struct Context {
  pub db_pool: DbPool,
}
impl juniper::Context for Context {}
impl Context {
  pub fn new(pool: DbPool) -> Self {
    Self { db_pool: pool }
  }
}

pub struct QueryRoot;
#[graphql_object(context = Context)]
impl QueryRoot {
  pub fn blog(context: &Context, name: String) -> ServiceResult<models::blog::Blog> {
    let blog = Blog::get_by_name(&context.db_pool, name)?;
    if let Some(blog) = blog {
      Ok(models::blog::Blog::from(blog))
    } else {
      Err(ServiceError::BadRequest("Invalid blog name".to_string()))
    }
  }
}

/* struct MutationRoot;
#[graphql_object(context = Context)]
impl MutationRoot {
  pub fn login() -> ServiceResult {}
  pub fn register() -> ServiceResult {}
} */

pub type Schema = RootNode<'static, QueryRoot, EmptyMutation<Context>, EmptySubscription<Context>>;

pub fn create_schema() -> Schema {
  Schema::new(QueryRoot {}, EmptyMutation::new(), EmptySubscription::new())
}
