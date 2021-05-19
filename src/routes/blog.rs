use actix_web::{web, HttpResponse, Responder};
use anyhow::anyhow;
use serde::Deserialize;
use sqlx::PgPool;

use tumblepub_db::models::{blog::Blog, post::PostContent};
use tumblepub_utils::{
  errors::{Result, TumblepubError},
  markdown::markdown_to_safe_html,
};

use crate::theme::{create_handlebars, ThemePost, ThemePostContent, ThemeVariables};

#[derive(Deserialize)]
pub struct BlogPath {
  pub blog: String,
}

pub async fn blog(path: web::Path<BlogPath>, pool: web::Data<PgPool>) -> Result<impl Responder> {
  let blog_name = &path.blog;
  let mut pool = pool.acquire().await.unwrap();

  // get blog and the 25 posts for this page
  let blog = Blog::find(&mut pool, (blog_name.clone(), None))
    .await
    .map_err(|e| TumblepubError::InternalServerError(e))?
    .ok_or_else(|| TumblepubError::NotFound)?;
  let posts = blog
    .posts(&mut pool, Some(25), Some(0))
    .await
    .map_err(|e| TumblepubError::InternalServerError(e))?;

  let hbs = create_handlebars()
    .map_err(|_| TumblepubError::InternalServerError(anyhow!("Could not create Handlebars")))?;

  let blog_title = blog.title.unwrap_or(blog.name);
  let vars = ThemeVariables {
    title: blog_title.clone(),
    description: blog.description,
    posts: posts
      .iter()
      .map(|post| ThemePost {
        created_at: post.created_at,
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
  config.service(web::resource("/@{blog}").route(web::get().to(blog)));
}
