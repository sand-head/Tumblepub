use actix_web::{web, Either, HttpResponse, Responder};
use serde::Deserialize;
use serde_json::json;

use sqlx::PgPool;
use tumblepub_ap::{activitypub_response, ActivityPub};
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::errors::Result;

use super::BlogPath;

#[derive(Deserialize)]
pub struct PagingParams {
  pub page: Option<bool>,
}

pub async fn get_ap_blog_outbox(
  path: web::Path<BlogPath>,
  params: web::Query<PagingParams>,
  pool: web::Data<PgPool>,
) -> Result<impl Responder> {
  let mut pool = pool.acquire().await.unwrap();
  let blog = Blog::find(&mut pool, (path.blog.clone(), None))
    .await
    .expect("could not retrieve blog from db");

  if let Some(blog) = blog {
    Ok(Either::A(
      if params.page.is_some() && params.page.unwrap() {
        // we paginate thru real posts here
        let posts = blog.posts(&mut pool, Some(20), Some(0)).await?;
        Either::A(activitypub_response(&(blog, posts).as_activitypub()?))
      } else {
        // we give a general overview of the outbox here
        let posts = blog.total_posts(&mut pool).await?;
        Either::B(activitypub_response(&(blog, posts).as_activitypub()?))
      },
    ))
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
