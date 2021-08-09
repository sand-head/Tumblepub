use actix_web::{web, HttpResponse};
use serde::Deserialize;
use sqlx::PgPool;

use tumblepub_ap::webfinger::{get_name_and_domain, Link, Resource};
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::{errors::Result, options::Options};

#[derive(Deserialize)]
pub struct WebfingerQuery {
  resource: String,
}

pub async fn webfinger(
  query: web::Query<WebfingerQuery>,
  pool: web::Data<PgPool>,
) -> Result<HttpResponse> {
  let (blog_name, domain) = get_name_and_domain(&query.resource)?;

  // we don't gotta deal with any domains that aren't ours
  let local_domain = Options::get().local_domain;
  if domain != local_domain {
    return Ok(HttpResponse::NotFound().finish());
  }

  let mut conn = pool.acquire().await.unwrap();
  let blog = Blog::find(&mut conn, (blog_name, None)).await;
  Ok(match blog {
    Ok(Some(blog)) => {
      let res = Resource {
        subject: format!("acct:{}@{}", blog.name, domain),
        aliases: vec![],
        links: vec![Link {
          rel: "self".to_string(),
          content_type: Some("application/activity+json".to_string()),
          href: Some(format!("https://{}/@{}", local_domain, blog.name)),
          titles: None,
          properties: None,
          template: None,
        }],
      };

      HttpResponse::Ok()
        .content_type("application/jrd+json")
        .json(res)
    }
    _ => HttpResponse::NotFound().finish(),
  })
}
