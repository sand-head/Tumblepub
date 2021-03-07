use crate::database::DbPool;
use juniper::{EmptyMutation, EmptySubscription, RootNode};

use self::query::Query;

pub mod models;
pub mod mutation;
pub mod query;

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

pub type Schema = RootNode<'static, Query, EmptyMutation<Context>, EmptySubscription<Context>>;

pub fn create_schema() -> Schema {
  Schema::new(Query {}, EmptyMutation::new(), EmptySubscription::new())
}
