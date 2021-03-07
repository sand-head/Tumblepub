use diesel::prelude::*;
use uuid::Uuid;

use crate::database::schema::posts;

#[derive(Identifiable, Queryable)]
#[table_name = "posts"]
pub struct Post {
  pub id: Uuid,
  pub blog_id: i64,
  // todo: determine how post content is stored
}
