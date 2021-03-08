use chrono::NaiveDateTime;

/// Represents a user's data and settings.
#[derive(sqlx::FromRow, Debug)]
pub struct User {
  pub id: i64,
  pub email: String,
  pub primary_blog: i64,
  pub hash: Vec<u8>,
  pub salt: String,
  pub created_at: NaiveDateTime,
  pub updated_at: NaiveDateTime,
  pub last_login_at: NaiveDateTime,
}

#[derive(Debug)]
pub struct InsertableUser {
  pub email: String,
  pub primary_blog: i64,
  pub hash: Vec<u8>,
  pub salt: String,
}
/*
impl User {
  pub fn get_by_id(pool: &DbPool, user_id: i64) -> ServiceResult<Option<User>> {
    use crate::database::schema::users::dsl::{id, users};

    users
      .filter(id.eq(user_id))
      .first::<User>(&connect(&pool)?)
      .optional()
      .map_err(|_| ServiceError::Unauthorized)
  }
}
 */
