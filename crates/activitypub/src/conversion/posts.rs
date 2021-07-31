use activitystreams::{
  activity::Create,
  chrono::FixedOffset,
  collection::{CollectionExt, OrderedCollection, OrderedCollectionPage},
  context,
  object::{ApObject, Note},
  prelude::*,
  public,
  url::Url,
};
use tumblepub_db::{
  models::{
    blog::Blog,
    post::{Post, PostContent},
  },
  PgConnection,
};
use tumblepub_utils::{errors::Result, markdown::markdown_to_safe_html, options::Options};

use crate::ActivityPub;

impl ActivityPub for Post {
  type ApType = ApObject<Note>;

  fn as_activitypub(&self) -> tumblepub_utils::errors::Result<Self::ApType> {
    let mut post = ApObject::new(Note::new());

    post
      .set_published(self.created_at.with_timezone(&FixedOffset::east(0)))
      // todo: change when post visibility is added
      .set_to(public())
      .set_content(
        self
          .content
          .0
          .iter()
          .map(|c| match c {
            PostContent::Markdown(markdown) => markdown_to_safe_html(markdown.to_owned()),
          })
          .collect::<Vec<_>>()
          .join("\n"),
      );

    Ok(post)
  }
}

pub async fn post_collection(conn: &mut PgConnection, blog: Blog) -> Result<OrderedCollection> {
  let total_posts = blog.total_posts(conn).await?;
  let local_domain = Options::get().local_domain;
  let mut collection = OrderedCollection::new();

  collection
    .set_context(context())
    .set_id(
      Url::parse(&format!(
        "https://{}/outbox/@{}.json",
        local_domain, blog.name
      ))
      .unwrap(),
    )
    .set_total_items(total_posts as u64)
    .set_first(
      Url::parse(&format!(
        "https://{}/outbox/@{}.json?page=true",
        local_domain, blog.name
      ))
      .unwrap(),
    );
  Ok(collection)
}

pub async fn post_collection_page(
  conn: &mut PgConnection,
  blog: Blog,
  page: i32,
) -> Result<OrderedCollectionPage> {
  let offset = page * 20;
  let posts = blog.posts(conn, Some(offset + 20), Some(offset)).await?;
  let local_domain = Options::get().local_domain;
  let mut collection = OrderedCollectionPage::new();

  collection
    .set_context(context())
    .set_id(
      Url::parse(&format!(
        "https://{}/outbox/@{}.json",
        local_domain, blog.name
      ))
      .unwrap(),
    )
    // todo: change when post visibility is added
    .set_to(public())
    .set_many_ordered_items(
      posts
        .iter()
        .map(|p: &Post| {
          let mut create = Create::new(
            Url::parse(&format!("https://{}/@{}.json", local_domain, blog.name)).unwrap(),
            p.as_activitypub()?.into_any_base().unwrap(),
          );
          create.set_published(p.created_at.with_timezone(&FixedOffset::east(0)));

          Ok(create.into_any_base().unwrap())
        })
        .collect::<Vec<Result<_>>>()
        .into_iter()
        .collect::<Result<Vec<_>>>()?,
    );
  Ok(collection)
}
