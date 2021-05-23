use actix_web::{web, Either, HttpResponse, Responder};
use serde::Deserialize;
use serde_json::json;
use sqlx::PgPool;

use tumblepub_ap::ActivityStreams;
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::{errors::TumblepubError, options::Options};

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

  let local_domain = Options::get().local_domain;
  if let Some(blog) = blog {
    Ok(Either::A(ActivityStreams(json!({
      "@context": "https://www.w3.org/ns/activitystreams",
      "type": "Person",
      "id": format!("{}/@{}.json", local_domain, blog.name),

      "inbox": format!("{}/inbox/@{}.json", local_domain, blog.name),
      "outbox": format!("{}/outbox/@{}.json", local_domain, blog.name),

      "preferredUsername": blog.name,
      "name": blog.title,
      "summary": blog.description,

      "publicKey": {
        "id": format!("{}/@{}.json#main-key", local_domain, blog.name),
        "owner": format!("{}/@{}.json", local_domain, blog.name),
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