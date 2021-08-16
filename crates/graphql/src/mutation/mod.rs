use std::convert::TryInto;

use async_graphql::{Context, ErrorExtensions, Object, Result};
use serde_json::json;
use sqlx::PgPool;

use tumblepub_ap::{
  models::activity::{Activity, ActivityKind, ActivityObject},
  ActivityPub,
};
use tumblepub_db::models::{
  blog::{Blog as DbBlog, NewBlog},
  post::{NewPost, Post as DbPost, PostContent as DbPostContent},
  user::User,
};
use tumblepub_utils::{crypto::KeyPair, errors::TumblepubError, jwt::Token, options::Options};

use self::{login::login, register::register};
use crate::models::{
  blog::Blog,
  posts::{Post, PostInput},
  AuthPayload,
};

mod login;
pub mod register;

pub struct Mutation;
#[Object]
impl Mutation {
  /// Authenticates against an existing user account, if one exists.
  pub async fn login(
    &self,
    context: &Context<'_>,
    email: String,
    password: String,
  ) -> Result<AuthPayload> {
    login(context, email, password).await
  }

  /// Registers a new user account.
  pub async fn register(
    &self,
    context: &Context<'_>,
    email: String,
    password: String,
    name: String,
  ) -> Result<AuthPayload> {
    if Options::get().single_user_mode {
      return Err(TumblepubError::BadRequest("Registration has been disabled.").extend());
    }

    let pool = context.data::<PgPool>()?;
    register(pool, email, password, name).await
  }

  /// Creates a new blog under the currently authenticated user.
  pub async fn create_blog(&self, ctx: &Context<'_>, name: String) -> Result<Blog> {
    let claims = ctx
      .data::<Token>()?
      .get_claims()
      .as_ref()
      .ok_or_else(|| TumblepubError::Unauthorized.extend())?;
    let mut txn = ctx.data::<PgPool>()?.begin().await.unwrap();

    let keypair =
      KeyPair::generate().map_err(|e| TumblepubError::InternalServerError(e).extend())?;
    let blog = DbBlog::create_new(
      &mut *txn,
      NewBlog {
        user_id: Some(claims.sub),
        uri: Option::<String>::None,
        name,
        domain: Option::<String>::None,
        is_primary: false,
        is_public: true,
        title: Option::<String>::None,
        description: Option::<String>::None,
        private_key: Some(keypair.private_key),
        public_key: keypair.public_key,
      },
    )
    .await
    .map_err(|e| TumblepubError::InternalServerError(e).extend())?;

    txn
      .commit()
      .await
      .map_err(|e| TumblepubError::InternalServerError(anyhow::format_err!(e)).extend())?;
    Ok(blog.into())
  }

  /// Modifies the description of the blog by the given name, provided that it exists and the current user is an admin of the blog.
  pub async fn set_blog_description(
    &self,
    ctx: &Context<'_>,
    name: String,
    description: String,
  ) -> Result<Blog> {
    let claims = ctx
      .data::<Token>()?
      .get_claims()
      .as_ref()
      .ok_or_else(|| TumblepubError::Unauthorized.extend())?;
    let mut conn = ctx.data::<PgPool>()?.acquire().await.unwrap();

    let user = User::get_by_id(&mut conn, claims.sub).await?;
    if user.is_some() {
      let blog = DbBlog::find(&mut conn, (name, None)).await?;
      if let Some(mut blog) = blog {
        if blog.user_id != Some(user.unwrap().id) {
          return Err(TumblepubError::Unauthorized.extend());
        }

        // ok! we can now set the blog's description
        blog.set_description(&mut conn, description).await?;
        // todo: deliver update to all followers,
        // instead of hardcoding it to update the main mastodon instance
        let local_domain = Options::get().local_domain;
        tumblepub_ap::deliver(
          Activity {
            context: json!("https://www.w3.org/ns/activitystreams"),
            kind: ActivityKind::Update,

            actor: format!("https://{}/@{}", local_domain, &blog.name),
            object: ActivityObject::Actor(blog.as_activitypub()?),

            published: None,
            to: vec!["https://mastodon.social/inbox".to_string()],
            cc: vec![],
          },
          blog.private_key.as_ref().unwrap(),
        )
        .await?;
        Ok(blog.into())
      } else {
        Err(TumblepubError::NotFound.extend())
      }
    } else {
      Err(TumblepubError::BadRequest("User is banned or does not exist.").extend())
    }
  }

  /// Creates a new post under the given blog.
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
    let blog = DbBlog::find(&mut pool, (blog_name, None))
      .await
      .map_err(|e| TumblepubError::InternalServerError(e).extend())?
      .ok_or_else(|| TumblepubError::BadRequest("the given blog does not exist").extend())?;

    // if the user does not own the blog, bad request
    if blog.user_id != Some(user.id) {
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
