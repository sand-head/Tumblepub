use actix_web::{guard, web, Either, HttpResponse, Responder};
use serde::Deserialize;
use sqlx::PgPool;

use tumblepub_ap::{
  activitypub_response,
  conversion::{blog::blog_to_ap, posts},
};
use tumblepub_db::models::{blog::Blog, post::Post};
use tumblepub_utils::errors::Result;
use uuid::Uuid;

use super::BlogPath;

mod inbox;
mod outbox;

/// A publically accessible JSON-LD document that provides information about a given blog.
pub async fn get_ap_blog(
  path: web::Path<BlogPath>,
  pool: web::Data<PgPool>,
) -> Result<impl Responder> {
  let mut pool = pool.acquire().await.unwrap();
  let blog = Blog::find(&mut pool, (path.blog.clone(), None))
    .await
    .expect("could not retrieve blog from db");

  if let Some(blog) = blog {
    Ok(Either::A(activitypub_response(&blog_to_ap(&blog)?)))
  } else {
    Ok(Either::B(
      HttpResponse::NotFound().body("The requested user does not exist."),
    ))
  }
}

pub async fn get_ap_blog_activity(pool: web::Data<PgPool>) -> Result<impl Responder> {
  let mut pool = pool.acquire().await.unwrap();

  Ok(HttpResponse::NotImplemented())
}

#[derive(Deserialize)]
pub struct BlogPostPath {
  pub blog: String,
  pub post_id: Uuid,
}

/// A publically accessible JSON-LD document that provides information about a given blog post.
pub async fn get_ap_blog_post(
  path: web::Path<BlogPostPath>,
  pool: web::Data<PgPool>,
) -> Result<impl Responder> {
  let mut pool = pool.acquire().await.unwrap();
  let blog = Blog::find(&mut pool, (path.blog.clone(), None)).await?;
  let post = Post::find(&mut pool, path.post_id).await?;

  if let Some(post) = post {
    Ok(Either::A(activitypub_response(&posts::post(
      path.blog.to_owned(),
      &post,
    )?)))
  } else {
    Ok(Either::B(
      HttpResponse::NotFound().body("The requested post does not exist."),
    ))
  }
}

pub fn routes(config: &mut web::ServiceConfig) {
  // get requests
  config.service(
    web::scope("/")
      .guard(
        guard::Any(guard::Header("Accept", "application/activity+json")).or(guard::Header(
          "Accept",
          r#"application/ld+json; profile="https://www.w3.org/ns/activitystreams""#,
        )),
      )
      .route("/@{blog}", web::get().to(get_ap_blog))
      .route(
        "/@{blog}/activity/{activity_id}",
        web::get().to(get_ap_blog_activity),
      )
      .route(
        "/@{blog}/followers",
        web::get().to(HttpResponse::NotImplemented),
      )
      .route("/@{blog}/outbox", web::get().to(outbox::get_ap_blog_outbox))
      .route("/@{blog}/posts/{post_id}", web::get().to(get_ap_blog_post)),
  );
  // post requests
  config.service(
    web::scope("/")
      .guard(
        guard::Any(guard::Header("Content-Type", "application/activity+json")).or(guard::Header(
          "Content-Type",
          r#"application/ld+json; profile="https://www.w3.org/ns/activitystreams""#,
        )),
      )
      .route("/@{blog}/inbox", web::post().to(inbox::post_ap_blog_inbox)),
  );
}
