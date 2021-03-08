use chrono::NaiveDateTime;

#[derive(sqlx::FromRow)]
pub struct Blog {
  pub id: i64,
  pub uri: Option<String>,
  pub name: String,
  pub domain: Option<String>,
  pub is_public: bool,
  pub title: Option<String>,
  pub description: Option<String>,
  pub created_at: NaiveDateTime,
  pub updated_at: NaiveDateTime,
}

pub struct InsertableBlog {
  pub uri: Option<String>,
  pub name: String,
  pub domain: Option<String>,
  pub is_public: bool,
  pub title: Option<String>,
  pub description: Option<String>,
}
/*
impl Blog {
  pub fn get_by_name<S>(pool: &DbPool, blog_name: S) -> ServiceResult<Option<Blog>>
  where
    S: Into<String>,
  {
    use crate::database::schema::blogs::dsl::{blogs, domain, name};
    blogs
      .filter(
        name
          .ilike::<String>(blog_name.into())
          .and(domain.eq::<Option<String>>(None)),
      )
      .first::<Blog>(&connect(&pool)?)
      .optional()
      .map_err(|_| ServiceError::Unauthorized)
  }

  pub fn list_by_user_id(pool: &DbPool, user_id: i64) -> ServiceResult<Vec<Blog>> {
    user_blogs::table
      .filter(user_blogs::user_id.eq(user_id))
      .inner_join(blogs::table.on(user_blogs::blog_id.eq(blogs::id)))
      .select(blogs::all_columns)
      .load::<Blog>(&connect(&pool)?)
      .map_err(|_| ServiceError::Unauthorized)
  }
}
 */
