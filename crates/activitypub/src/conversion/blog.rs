use serde_json::json;
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::{description_to_html, errors::Result, options::Options};

use crate::models::actor::{Actor, ActorKind, PublicKey};

pub fn blog_to_ap(blog: &Blog) -> Result<Actor> {
  let local_domain = Options::get().local_domain;
  // https is assumed here because you really gotta use https
  // I could probably switch protocols based on request protocol
  // but that'd be adding support for http... just reverse proxy
  let actor = Actor {
    context: json!("https://www.w3.org/ns/activitystreams"),
    kind: ActorKind::Person,

    id: format!("https://{}/@{}", local_domain, blog.name),
    name: blog.title.as_ref().unwrap_or(&blog.name).to_owned(),
    preferred_username: blog.name.to_owned(),
    summary: blog
      .description
      .as_ref()
      .map(|desc| description_to_html(desc.to_owned())),
    published: blog.created_at.naive_utc(),

    inbox: format!("https://{}/@{}/inbox", local_domain, blog.name),
    outbox: format!("https://{}/@{}/outbox", local_domain, blog.name),
    followers: Some(format!("https://{}/@{}/followers", local_domain, blog.name)),
    following: Some(format!("https://{}/@{}/following", local_domain, blog.name)),

    public_key: PublicKey {
      id: format!("https://{}/@{}#main-key", local_domain, blog.name),
      owner: format!("https://{}/@{}", local_domain, blog.name),
      public_key_pem: String::from_utf8(blog.public_key.to_owned()).unwrap(),
    },
  };
  Ok(actor)
}
