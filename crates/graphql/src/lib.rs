use async_graphql::*;
use sqlx::PgPool;

use self::{mutation::Mutation, query::Query};

pub mod models;
pub mod mutation;
pub mod query;

pub type TumblepubSchema = Schema<Query, Mutation, EmptySubscription>;

pub fn create_schema(db_pool: PgPool) -> Schema<Query, Mutation, EmptySubscription> {
  Schema::build(Query, Mutation, EmptySubscription)
    .data(db_pool)
    .finish()
}

#[cfg(test)]
mod tests {
  #[test]
  fn it_works() {
    assert_eq!(2 + 2, 4);
  }
}
