use actix_web::{web, HttpResponse, Responder};
use anyhow::anyhow;
use chrono::Local;
use serde::Deserialize;
use sqlx::{PgConnection, PgPool};

use tumblepub_db::models::{blog::Blog, post::PostContent, user::User};
use tumblepub_utils::{
  description_to_html,
  errors::{Result, TumblepubError},
  markdown::markdown_to_safe_html,
  options::Options,
};

use crate::theme::{create_handlebars, ThemePost, ThemePostContent, ThemeVariables};

#[derive(Deserialize)]
pub struct BlogPath {
  pub blog: String,
}

/// Displays the primary blog of the first user, for single user mode.
async fn single_blog_route(pool: web::Data<PgPool>) -> Result<impl Responder> {
  let mut conn = pool.acquire().await.unwrap();
  let first_user = User::get_by_id(&mut conn, 1)
    .await
    .map_err(TumblepubError::InternalServerError)?
    .ok_or(TumblepubError::NotFound)?;
  let blog = first_user
    .primary_blog(&mut conn)
    .await
    .map_err(TumblepubError::InternalServerError)?;

  display_blog(&mut conn, blog).await
}

async fn blog_route(path: web::Path<BlogPath>, pool: web::Data<PgPool>) -> Result<impl Responder> {
  let blog_name = &path.blog;
  let mut conn = pool.acquire().await.unwrap();
  let blog = Blog::find(&mut conn, (blog_name.clone(), None))
    .await
    .map_err(TumblepubError::InternalServerError)?
    .ok_or(TumblepubError::NotFound)?;

  display_blog(&mut conn, blog).await
}

async fn display_blog(conn: &mut PgConnection, blog: Blog) -> Result<impl Responder> {
  // get blog and the 25 posts for this page
  let posts = blog
    .posts(conn, Some(25), Some(0))
    .await
    .map_err(TumblepubError::InternalServerError)?;

  let hbs = create_handlebars()
    .map_err(|_| TumblepubError::InternalServerError(anyhow!("Could not create Handlebars")))?;

  let blog_title = blog.title.unwrap_or(blog.name);
  let vars = ThemeVariables {
    title: blog_title,
    description: blog.description.map(description_to_html),
    posts: posts
      .iter()
      .map(|post| ThemePost {
        created_at: post.created_at.with_timezone(&Local),
        content: post
          .content
          .iter()
          .map(|content| match content {
            PostContent::Markdown(markdown) => ThemePostContent::Markdown {
              content: markdown_to_safe_html(markdown.to_owned()),
            },
          })
          .collect(),
      })
      .collect(),

    previous_page: None,
    next_page: None,
  };

  Ok(
    HttpResponse::Ok().body(
      hbs
        .render("default", &vars)
        .map_err(|_| TumblepubError::InternalServerError(anyhow!("Could not render blog")))?,
    ),
  )
}

pub fn routes(config: &mut web::ServiceConfig) {
  if Options::get().single_user_mode {
    config.service(web::resource("/").route(web::get().to(single_blog_route)));
  }

  config.service(web::resource("/@{blog}").route(web::get().to(blog_route)));
}
