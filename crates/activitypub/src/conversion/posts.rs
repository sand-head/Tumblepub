use activitystreams::{
  collection::{CollectionExt, OrderedCollection},
  object::{ApObject, Page},
  url::Url,
};
use tumblepub_db::models::post::Post;
use tumblepub_utils::options::Options;

use crate::ActivityPub;

impl ActivityPub for Post {
  type ApType = ApObject<Page>;

  fn as_activitypub(&self) -> tumblepub_utils::errors::Result<Self::ApType> {
    todo!()
  }
}

impl ActivityPub for Vec<Post> {
  type ApType = OrderedCollection;

  fn as_activitypub(&self) -> tumblepub_utils::errors::Result<Self::ApType> {
    let local_domain = Options::get().local_domain;
    let mut collection = OrderedCollection::new();

    collection
      .set_total_items(self.len() as u64)
      .set_many_items(
        self
          .iter()
          .map(|p| {
            Url::parse(&format!(
              "https://{}/@{}/posts/{}.json",
              local_domain, "blog go here", p.id
            ))
            .unwrap()
          })
          .collect::<Vec<_>>(),
      );
    Ok(collection)
  }
}
