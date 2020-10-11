use crate::schema::blogs;

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
