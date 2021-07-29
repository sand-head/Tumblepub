use actix_web::{web, Either, HttpResponse, Responder};
use serde_json::json;

use sqlx::PgPool;
use tumblepub_ap::{activitypub_response, ActivityPub};
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::errors::Result;

use super::BlogPath;

pub async fn get_ap_blog_outbox(
  path: web::Path<BlogPath>,
  pool: web::Data<PgPool>,
) -> Result<impl Responder> {
  let mut pool = pool.acquire().await.unwrap();
  let blog = Blog::find(&mut pool, (path.blog.clone(), None))
    .await
    .expect("could not retrieve blog from db");

  if let Some(blog) = blog {
    let posts = blog.posts(&mut pool, Some(20), Some(0)).await?;
    Ok(Either::A(activitypub_response(&posts.as_activitypub()?)))
  } else {
    Ok(Either::B(
      HttpResponse::NotFound().body("The requested user does not exist."),
    ))
  }
}

pub async fn post_ap_blog_outbox() -> Result<impl Responder> {
  // todo: implement posting outbox
  Ok(activitypub_response(&json!({})))
}
