use actix_web::{web, Either, HttpResponse, Responder};
use serde::Deserialize;
use sqlx::PgPool;

use tumblepub_ap::{activitypub_response, ActivityPub};
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::errors::TumblepubError;

mod inbox;
mod outbox;

#[derive(Deserialize)]
pub struct BlogPath {
  pub blog: String,
}

/// A publically accessible JSON-LD document that provides information about a given blog.
pub async fn get_ap_blog(
  path: web::Path<BlogPath>,
  pool: web::Data<PgPool>,
) -> Result<impl Responder, TumblepubError> {
  let mut pool = pool.acquire().await.unwrap();
  let blog = Blog::find(&mut pool, (path.blog.clone(), None))
    .await
    .expect("could not retrieve blog from db");

  if let Some(blog) = blog {
    Ok(Either::A(activitypub_response(&blog.as_activitypub()?)))
  } else {
    Ok(Either::B(
      HttpResponse::NotFound().body("The requested user does not exist."),
    ))
  }
}

pub fn routes(config: &mut web::ServiceConfig) {
  config.service(web::resource("/@{blog}.json").route(web::get().to(get_ap_blog)));
  config.service(
    web::resource("/inbox/@{blog}.json")
      .route(web::get().to(inbox::get_ap_blog_inbox))
      .route(web::post().to(inbox::post_ap_blog_inbox)),
  );
  config.service(
    web::resource("/outbox/@{blog}.json")
      .route(web::get().to(outbox::get_ap_blog_outbox))
      .route(web::post().to(outbox::post_ap_blog_outbox)),
  );
}
