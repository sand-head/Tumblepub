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

pub fn post(blog_name: String, post: &Post) -> Result<ApObject<Note>> {
  let local_domain = Options::get().local_domain;
  let mut object = ApObject::new(Note::new());

  object
    .set_id(
      Url::parse(&format!(
        "https://{}/@{}/posts/{}",
        local_domain, blog_name, post.id
      ))
      .unwrap(),
    )
    .set_published(post.created_at.with_timezone(&FixedOffset::east(0)))
    // todo: change when post visibility is added
    .set_to(public())
    .set_attributed_to(Url::parse(&format!("https://{}/@{}", local_domain, blog_name)).unwrap())
    .set_content(
      post
        .content
        .0
        .iter()
        .map(|c| match c {
          PostContent::Markdown(markdown) => markdown_to_safe_html(markdown.to_owned()),
        })
        .collect::<Vec<_>>()
        .join("\n"),
    );

  Ok(object)
}

pub async fn post_collection(conn: &mut PgConnection, blog: Blog) -> Result<OrderedCollection> {
  let total_posts = blog.total_posts(conn).await?;
  let local_domain = Options::get().local_domain;
  let mut collection = OrderedCollection::new();

  collection
    .set_context(context())
    .set_id(Url::parse(&format!("https://{}/@{}/outbox", local_domain, blog.name)).unwrap())
    .set_total_items(total_posts as u64)
    .set_first(
      Url::parse(&format!(
        "https://{}/@{}/outbox?page=true",
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
    .set_id(Url::parse(&format!("https://{}/@{}/outbox", local_domain, blog.name)).unwrap())
    .set_many_ordered_items(
      posts
        .iter()
        .map(|p: &Post| {
          let mut create = Create::new(
            Url::parse(&format!("https://{}/@{}", local_domain, blog.name)).unwrap(),
            post(blog.name.to_owned(), p)?.into_any_base().unwrap(),
          );
          create
            // todo: change when post visibility is added
            .set_to(public())
            .set_published(p.created_at.with_timezone(&FixedOffset::east(0)));

          Ok(create.into_any_base().unwrap())
        })
        .collect::<Vec<Result<_>>>()
        .into_iter()
        .collect::<Result<Vec<_>>>()?,
    );
  Ok(collection)
}
