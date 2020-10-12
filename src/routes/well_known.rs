use std::{collections::HashMap, env};

use actix_web::{get, web, HttpResponse, Responder};
use lazy_static::lazy_static;
use regex::Regex;
use serde::{Deserialize, Serialize};

use crate::models::Blog;

use super::AppState;

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

#[get("/.well-known/webfinger")]
pub async fn webfinger(
  query: web::Query<WebfingerReq>,
  data: web::Data<AppState<'_>>,
) -> impl Responder {
  let resource = match &query.resource {
    Some(resource) => resource,
    None => return HttpResponse::BadRequest().finish()
  };
  let parsed = WEBFINGER_URI.captures(resource);
  let captures = match parsed {
    Some(captures) => captures,
    None => return HttpResponse::BadRequest().finish()
  };

  let local_domain = env::var("LOCAL_DOMAIN").expect("Environment variable LOCAL_DOMAIN must be set");
  let blog_name = captures.get(1)
    .map_or("", |c| c.as_str())
    .to_string();
  let domain = captures.get(2)
    .map_or("", |c| c.as_str())
    .to_string();

  // we don't gotta deal with any domains that aren't ours
  if domain != local_domain {
    return HttpResponse::NotFound().finish();
  }

  let conn = data.pool.get()
    .expect("Could not get database connection from pool");
  let blog = web::block(move || Blog::get_by_name(&conn, blog_name))
    .await
    .map_err(|e| {
      eprintln!("{}", e);
      HttpResponse::InternalServerError().finish()
    })
    .expect("Could not get blog from database");

  match blog {
    Some(blog) => {
      let res = WebfingerRes {
        subject: format!("acct:{}@{}", blog.name, domain),
        aliases: vec![],
        links: vec![],
      };

      HttpResponse::Ok()
        .content_type("application/jrd+json")
        .json(res)
    }
    None => HttpResponse::NotFound().finish(),
  }
}
