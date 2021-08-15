use actix_web::{web, Either, HttpResponse, Responder};
use serde::Deserialize;

use sqlx::PgPool;
use tumblepub_ap::{
  activitypub_response,
  conversion::posts::{post_collection, post_collection_page},
};
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::errors::Result;

use crate::routes::BlogPath;

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
        Either::A(activitypub_response(
          &post_collection_page(&mut pool, blog, 0).await?,
        ))
      } else {
        // we give a general overview of the outbox here
        Either::B(activitypub_response(
          &post_collection(&mut pool, blog).await?,
        ))
      },
    ))
  } else {
    Ok(Either::B(
      HttpResponse::NotFound().body("The requested user does not exist."),
    ))
  }
}
