use actix_web::{web, Either, HttpResponse, Responder};
use serde::Deserialize;
use serde_json::json;
use sqlx::PgPool;
use tumblepub_ap::ActivityStreams;
use tumblepub_db::models::blog::Blog;

use crate::errors::ServiceResult;
use crate::LOCAL_DOMAIN;

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
) -> ServiceResult<impl Responder> {
  let blog = Blog::find(&pool, (path.blog.clone(), None))
    .await
    .expect("could not retrieve blog from db");

  if let Some(blog) = blog {
    Ok(Either::A(ActivityStreams(json!({
      "@context": "https://www.w3.org/ns/activitystreams",
      "type": "Person",
      "id": format!("{}/@{}.json", LOCAL_DOMAIN.as_str(), blog.name),

      "inbox": format!("{}/inbox/@{}.json", LOCAL_DOMAIN.as_str(), blog.name),
      "outbox": format!("{}/outbox/@{}.json", LOCAL_DOMAIN.as_str(), blog.name),

      "preferredUsername": blog.name,
      "name": blog.title,
      "summary": blog.description,

      "publicKey": {
        "id": format!("{}/@{}.json#main-key", LOCAL_DOMAIN.as_str(), blog.name),
        "owner": format!("{}/@{}.json", LOCAL_DOMAIN.as_str(), blog.name),
        "publicKeyPem": blog.public_key
      }
    }))))
  } else {
    Ok(Either::B(
      HttpResponse::NotFound().body("The requested user does not exist."),
    ))
  }
}

pub fn routes(config: &mut web::ServiceConfig) {
  config.service(
    web::scope("/")
      .route("/@{blog}.json", web::get().to(get_ap_blog))
      .route(
        "/inbox/@{blog}.json",
        web::get().to(inbox::get_ap_blog_inbox),
      )
      .route(
        "/inbox/@{blog}.json",
        web::post().to(inbox::post_ap_blog_inbox),
      )
      .route(
        "/outbox/@{blog}.json",
        web::get().to(outbox::get_ap_blog_outbox),
      )
      .route(
        "/outbox/@{blog}.json",
        web::post().to(outbox::post_ap_blog_outbox),
      ),
  );
}
