use std::collections::HashMap;

use once_cell::sync::Lazy;
use regex::Regex;
use serde::{Deserialize, Serialize};
use tumblepub_utils::{
  errors::{Result, TumblepubError},
  HTTP_CLIENT,
};

static WEBFINGER_URI: Lazy<Regex> = Lazy::new(|| Regex::new("^acct:([a-z0-9_]*)@(.*)$").unwrap());

#[derive(Deserialize, Serialize)]
pub struct Link {
  /// The relation of the link.
  pub rel: String,
  #[serde(rename = "type")]
  /// The content type of the link's resource.
  pub content_type: Option<String>,
  /// The link itself.
  pub href: Option<String>,
  /// Zero or more key-value pairs, where the key is a language tag ("en-us", "fr", etc.)
  /// or "und", and the value is a title in that given language.
  /// https://tools.ietf.org/html/rfc7033#section-4.4.4.4
  pub titles: Option<HashMap<String, String>>,
  /// Zero or more key-value pairs that convey additional information about the relation.
  /// https://tools.ietf.org/html/rfc7033#section-4.4.4.5
  pub properties: Option<HashMap<String, String>>,
  /// A concept carried over from OStatus, and is used by Mastodon to notify other instances
  /// of a "follow" or "subscribe" authorization page, like so:
  /// ```json
  /// {
  ///   "rel": "http://ostatus.org/schema/1.0/subscribe",
  ///   "template": "https://mastodon.social/authorize_interaction?uri={uri}"
  /// }
  /// ```
  pub template: Option<String>,
}

#[derive(Deserialize, Serialize)]
pub struct Resource {
  pub subject: String,
  pub aliases: Vec<String>,
  pub links: Vec<Link>,
}

/// Obtains the blog name and domain from a Webfinger `acct` resource.
pub fn get_name_and_domain(acct: &str) -> Result<(String, String)> {
  let captures = match WEBFINGER_URI.captures(acct) {
    Some(captures) => Ok(captures),
    None => Err(TumblepubError::NotFound),
  }?;

  let blog_name = captures
    .get(1)
    .ok_or(TumblepubError::BadRequest("no blog name found"))?
    .as_str()
    .to_string();
  let domain = captures
    .get(2)
    .ok_or(TumblepubError::BadRequest("no domain found"))?
    .as_str()
    .to_string();

  Ok((blog_name, domain))
}

/// Retrieves a [resource](Resource) from a foreign instance using Webfinger.
pub async fn get_resource(acct: String) -> Result<Resource> {
  let (_, domain) = get_name_and_domain(&acct)?;

  // webfinger the domain for the account
  let request = HTTP_CLIENT
    .get(format!(
      "https://{}/.well-known/webfinger?resource={}",
      domain, acct
    ))
    .header("Accept", "application/jrd+json")
    .build()?;
  let resource: Resource = HTTP_CLIENT.execute(request).await?.json().await?;

  Ok(resource)
}
