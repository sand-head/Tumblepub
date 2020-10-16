use diesel::prelude::*;

use crate::database::schema::blogs;

#[derive(Identifiable, Queryable)]
#[table_name = "blogs"]
pub struct Blog {
  pub id: i64,
  pub uri: Option<String>,
  pub name: String,
  pub domain: Option<String>,

  pub title: Option<String>,
  pub description: Option<String>,
}

impl Blog {
  pub fn get_by_name<S>(conn: &PgConnection, name: S) -> QueryResult<Option<Blog>>
  where
    S: Into<String>,
  {
    use crate::database::schema::blogs::dsl;
    dsl::blogs
      .filter(
        dsl::name
          .ilike::<String>(name.into())
          .and(dsl::domain.eq::<Option<String>>(None)),
      )
      .first::<Blog>(conn)
      .optional()
  }
}
