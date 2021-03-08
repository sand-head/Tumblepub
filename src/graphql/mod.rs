use juniper::{EmptyMutation, EmptySubscription, RootNode};
use sqlx::PgPool;

use self::{mutation::Mutation, query::Query};

pub mod models;
pub mod mutation;
pub mod query;

#[derive(Clone)]
pub struct Context {
  pub db_pool: PgPool,
}
impl juniper::Context for Context {}
impl Context {
  pub fn new(pool: PgPool) -> Self {
    Self { db_pool: pool }
  }
}

pub type Schema = RootNode<'static, Query, Mutation, EmptySubscription<Context>>;

pub fn create_schema() -> Schema {
  Schema::new(Query, Mutation, EmptySubscription::<Context>::new())
}
