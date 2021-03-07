pub use blog::Blog;
pub use post::Post;
pub use user::User;
pub use user_blogs::UserBlogs;

mod blog;
mod post;
mod user;
mod user_blogs;

trait CRUD {}
