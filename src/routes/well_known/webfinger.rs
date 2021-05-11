use std::collections::HashMap;

use actix_web::{web, HttpResponse};
use lazy_static::lazy_static;
use regex::Regex;
use serde::{Deserialize, Serialize};
use sqlx::PgPool;
use tumblepub_db::models::blog::Blog;

use crate::{
  errors::{ServiceError, ServiceResult},
  LOCAL_DOMAIN,
};

lazy_static! {
  static ref WEBFINGER_URI: Regex = Regex::new("^acct:([a-z0-9_]*)@(.*)$").unwrap();
}

#[derive(Deserialize)]
pub struct WebfingerReq {
  resource: Option<String>,
}

#[derive(Serialize)]
pub struct WebfingerLink {
  /// The relation of the link.
  rel: String,
  #[serde(rename = "type")]
  /// The content type of the link's resource.
  content_type: Option<String>,
  /// The link itself.
  href: Option<String>,
  /// Zero or more key-value pairs, where the key is a language tag ("en-us", "fr", etc.)
  /// or "und", and the value is a title in that given language.
  /// https://tools.ietf.org/html/rfc7033#section-4.4.4.4
  titles: Option<HashMap<String, String>>,
  /// Zero or more key-value pairs that convey additional information about the relation.
  /// https://tools.ietf.org/html/rfc7033#section-4.4.4.5
  properties: Option<HashMap<String, String>>,
  /// A concept carried over from OStatus, and is used by Mastodon to notify other instances
  /// of a "follow" or "subscribe" authorization page, like so:
  /// ```json
  /// {
  ///   "rel": "http://ostatus.org/schema/1.0/subscribe",
  ///   "template": "https://mastodon.social/authorize_interaction?uri={uri}"
  /// }
  /// ```
  template: Option<String>,
}

#[derive(Serialize)]
pub struct WebfingerRes {
  subject: String,
  aliases: Vec<String>,
  links: Vec<WebfingerLink>,
}

pub async fn webfinger(
  query: web::Query<WebfingerReq>,
  pool: web::Data<PgPool>,
) -> ServiceResult<HttpResponse> {
  let resource = match &query.resource {
    Some(res) => res,
    None => return Err(ServiceError::BadRequest("Invalid resource".to_string())),
  };
  let captures = match WEBFINGER_URI.captures(resource) {
    Some(captures) => captures,
    None => return Err(ServiceError::BadRequest("Invalid resource".to_string())),
  };

  let blog_name = captures.get(1).map_or("", |c| c.as_str()).to_string();
  let domain = captures.get(2).map_or("", |c| c.as_str()).to_string();

  // we don't gotta deal with any domains that aren't ours
  if domain != LOCAL_DOMAIN.as_str() {
    return Ok(HttpResponse::NotFound().finish());
  }

  let blog = Blog::find(&pool, (blog_name, None))
    .await
    .expect("could not find blog");
  Ok(match blog {
    Some(blog) => {
      let res = WebfingerRes {
        subject: format!("acct:{}@{}", blog.name, domain),
        aliases: vec![],
        links: vec![WebfingerLink {
          rel: "self".to_string(),
          content_type: Some("application/activity+json".to_string()),
          href: Some(format!("https://{}/@{}.json", LOCAL_DOMAIN.as_str(), blog.name).to_string()),
          titles: None,
          properties: None,
          template: None,
        }],
      };

      HttpResponse::Ok()
        .content_type("application/jrd+json")
        .json(res)
    }
    None => HttpResponse::NotFound().finish(),
  })
}
