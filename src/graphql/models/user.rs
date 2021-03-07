use chrono::NaiveDateTime;
use juniper::GraphQLObject;

#[derive(GraphQLObject)]
/// Information about a user
pub struct User {
  /// The user's username, based on the username of their primary blog
  name: String,
  /// The date and time that the user first joined
  joined_at: NaiveDateTime,
}
