use std::convert::TryInto;

use async_graphql::{Context, ErrorExtensions, Object, Result};
use sqlx::PgPool;

use tumblepub_db::models::{
  blog::Blog,
  post::{NewPost, Post as DbPost, PostContent as DbPostContent},
  user::User,
  user_blogs::UserBlogs,
};
use tumblepub_utils::{errors::TumblepubError, jwt::Token};

use self::{login::login, register::register};
use super::models::user::UserAuthPayload;
use crate::models::posts::{Post, PostInput};

mod login;
pub mod register;

pub struct Mutation;
#[Object]
impl Mutation {
  pub async fn login(
    &self,
    context: &Context<'_>,
    email: String,
    password: String,
  ) -> Result<UserAuthPayload> {
    login(context, email, password).await
  }

  pub async fn register(
    &self,
    context: &Context<'_>,
    email: String,
    password: String,
    name: String,
  ) -> Result<UserAuthPayload> {
    let pool = context.data::<PgPool>()?;
    register(pool, email, password, name).await
  }

  pub async fn create_post(
    &self,
    ctx: &Context<'_>,
    #[graphql(desc = "The name of the local blog.")] blog_name: String,
    post: PostInput,
  ) -> Result<Post> {
    // todo: make this SO much better
    // too many calls to the database imo

    let token = ctx.data::<Token>()?;
    let mut pool = ctx.data::<PgPool>()?.acquire().await.unwrap();

    // get both the current user and the requested blog
    let user = User::get_by_token(&mut pool, token)
      .await
      .map_err(|_| TumblepubError::Unauthorized.extend())?
      .ok_or_else(|| TumblepubError::Unauthorized.extend())?;
    let blog = Blog::find(&mut pool, (blog_name, None))
      .await
      .map_err(|e| TumblepubError::InternalServerError(e).extend())?
      .ok_or_else(|| TumblepubError::BadRequest("the given blog does not exist").extend())?;

    // if the user does not own the blog, bad request
    if user.primary_blog != blog.id && !UserBlogs::exists(&mut pool, user.id, blog.id).await? {
      return Err(
        TumblepubError::BadRequest("the given blog does not belong to the current user").extend(),
      );
    }

    // create the post in the database
    let post_content: anyhow::Result<Vec<DbPostContent>> = post
      .content
      .iter()
      .map(|input| -> anyhow::Result<DbPostContent> { input.try_into() })
      .collect();
    let post = DbPost::create_new(
      &mut pool,
      NewPost {
        blog_id: blog.id,
        content: post_content.map_err(|_| {
          TumblepubError::BadRequest("saving one or more chunks of the post failed").extend()
        })?,
      },
    )
    .await
    .map_err(|e| TumblepubError::InternalServerError(e).extend())?;
    Ok(post.into())
  }
}
