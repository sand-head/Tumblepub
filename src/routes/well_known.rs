use std::collections::HashMap;

use actix_web::{get, web, HttpResponse, Responder};
use serde::{Deserialize, Serialize};

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
pub async fn webfinger(query: web::Query<WebfingerReq>) -> impl Responder {
  match &query.resource {
    Some(resource) => {
      println!("requested resource: {}", resource);

      // todo: get from database
      let res = WebfingerRes {
        subject: resource.clone(),
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
