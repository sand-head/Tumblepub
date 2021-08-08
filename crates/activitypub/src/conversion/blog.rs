use serde_json::json;
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::{description_to_html, options::Options};

use crate::{
  models::actor::{Actor, ActorKind, PublicKey},
  ActivityPub,
};

impl ActivityPub for Blog {
  type ApType = Actor;

  fn as_activitypub(&self) -> tumblepub_utils::errors::Result<Self::ApType> {
    let local_domain = Options::get().local_domain;
    // https is assumed here because you really gotta use https
    // I could probably switch protocols based on request protocol
    // but that'd be adding support for http... just reverse proxy
    let actor = Actor {
      context: json!("https://www.w3.org/ns/activitystreams"),
      kind: ActorKind::Person,

      id: format!("https://{}/@{}", local_domain, self.name),
      name: self.title.as_ref().unwrap_or(&self.name).to_owned(),
      preferred_username: self.name.to_owned(),
      summary: self
        .description
        .as_ref()
        .map(|desc| description_to_html(desc.to_owned())),
      published: self.created_at.naive_utc(),

      inbox: format!("https://{}/@{}/inbox", local_domain, self.name),
      outbox: format!("https://{}/@{}/outbox", local_domain, self.name),
      followers: None,
      following: None,

      public_key: PublicKey {
        id: format!("https://{}/@{}#main-key", local_domain, self.name),
        owner: format!("https://{}/@{}", local_domain, self.name),
        public_key_pem: String::from_utf8(self.public_key.to_owned()).unwrap(),
      },
    };
    Ok(actor)
  }
}
