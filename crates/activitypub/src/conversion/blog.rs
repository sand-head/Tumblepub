use activitystreams::{
  actor::{ApActor, ApActorExt, Person},
  chrono::FixedOffset,
  context,
  object::ObjectExt,
  prelude::BaseExt,
  url::Url,
};
use activitystreams_ext::Ext1;
use tumblepub_db::models::blog::Blog;
use tumblepub_utils::options::Options;

use crate::{extensions::public_key::PublicKey, ActivityPub, ApBlog};

impl ActivityPub for Blog {
  type ApType = ApBlog;

  fn as_activitypub(&self) -> tumblepub_utils::errors::Result<Self::ApType> {
    let local_domain = Options::get().local_domain;
    // https is assumed here because you really gotta use https
    // I could probably switch protocols based on request protocol
    // but that'd be adding support for http... just reverse proxy
    let mut blog = ApActor::new(
      Url::parse(&format!("https://{}/@{}/inbox", local_domain, self.name)).unwrap(),
      Person::new(),
    );

    blog
      .set_context(context())
      .set_id(Url::parse(&format!("https://{}/@{}", local_domain, self.name)).unwrap())
      .set_published(self.created_at.with_timezone(&FixedOffset::east(0)))
      .set_name(self.title.as_ref().unwrap_or(&self.name).to_owned())
      .set_preferred_username(self.name.to_owned())
      .set_outbox(Url::parse(&format!("https://{}/@{}/outbox", local_domain, self.name)).unwrap());

    if let Some(description) = self.description.to_owned() {
      blog.set_summary(description);
    }

    Ok(Ext1::new(
      blog,
      PublicKey {
        public_key: crate::models::actor::PublicKey {
          id: format!("https://{}/@{}#main-key", local_domain, self.name),
          owner: format!("https://{}/@{}", local_domain, self.name),
          public_key_pem: String::from_utf8(self.public_key.to_owned()).unwrap(),
        },
      },
    ))
  }
}
