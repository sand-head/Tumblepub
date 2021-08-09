use once_cell::sync::Lazy;
use regex::Regex;
use serde_json::json;
use tumblepub_db::{
  models::{
    blog::Blog,
    post::{Post, PostContent},
  },
  PgConnection,
};
use tumblepub_utils::{errors::Result, markdown::markdown_to_safe_html, options::Options};

use crate::models::{
  activity::{Activity, ActivityKind, ActivityObject},
  collection::{Collection, CollectionItem, CollectionKind},
  object::{Object, ObjectKind},
};

/// Captures Markdown images and headings of any size.
/// Used in determining whether a post should be considered an article in ActivityPub for compatibility.
static ARTICLE_KIND_REGEX: Lazy<Regex> = Lazy::new(|| {
  Regex::new(r#"!\[([^]]*)\]\(([^)]+)\)|^#*# (.+)"#).expect("could not compile article regex")
});

pub fn post(blog_name: String, post: &Post) -> Result<Object> {
  let local_domain = Options::get().local_domain;

  // flatten the post's content
  let content = post
    .content
    .0
    .iter()
    .map(|c| match c {
      PostContent::Markdown(markdown) => markdown.to_owned(),
    })
    .collect::<Vec<_>>()
    .join("\n");

  let object = Object {
    context: json!("https://www.w3.org/ns/activitystreams"),
    kind: if ARTICLE_KIND_REGEX.captures(&content).is_some() {
      // if the post contains image tags or headings
      // then we'll send it off as an article
      ObjectKind::Article
    } else {
      ObjectKind::Note
    },

    id: format!("https://{}/@{}/posts/{}", local_domain, blog_name, post.id),
    attributed_to: format!("https://{}/@{}", local_domain, blog_name),
    content: markdown_to_safe_html(content),

    published: Some(post.created_at.naive_utc()),
    // todo: change when post visibility is added
    to: vec!["https://www.w3.org/ns/activitystreams#Public".to_string()],
    cc: vec![],
  };

  Ok(object)
}

pub async fn post_collection(conn: &mut PgConnection, blog: Blog) -> Result<Collection> {
  let total_posts = blog.total_posts(conn).await?;
  let local_domain = Options::get().local_domain;

  let collection = Collection {
    context: json!("https://www.w3.org/ns/activitystreams"),
    kind: CollectionKind::OrderedCollection,

    id: format!("https://{}/@{}/outbox", local_domain, blog.name),
    total_items: total_posts as i64,
    items: vec![],
    ordered_items: vec![],

    first: Some(format!(
      "https://{}/@{}/outbox?page=true",
      local_domain, blog.name
    )),
  };

  Ok(collection)
}

pub async fn post_collection_page(
  conn: &mut PgConnection,
  blog: Blog,
  page: i32,
) -> Result<Collection> {
  let offset = page * 20;
  let total_posts = blog.total_posts(conn).await?;
  let posts = blog.posts(conn, Some(offset + 20), Some(offset)).await?;
  let local_domain = Options::get().local_domain;

  let collection = Collection {
    context: json!("https://www.w3.org/ns/activitystreams"),
    kind: CollectionKind::OrderedCollectionPage,

    id: format!("https://{}/@{}/outbox", local_domain, blog.name),
    total_items: total_posts as i64,
    items: vec![],
    ordered_items: posts
      .iter()
      .map(|p: &Post| {
        let create = Activity {
          context: json!("https://www.w3.org/ns/activitystreams"),
          kind: ActivityKind::Create,

          actor: format!("https://{}/@{}", local_domain, blog.name),
          object: ActivityObject::Object(post(blog.name.to_owned(), p)?),

          published: Some(p.created_at.naive_utc()),
          // todo: change when post visibility is added
          to: vec!["https://www.w3.org/ns/activitystreams#Public".to_string()],
          cc: vec![],
        };

        Ok(CollectionItem::Activity(create))
      })
      .collect::<Vec<Result<_>>>()
      .into_iter()
      .collect::<Result<Vec<_>>>()?,

    first: None,
  };

  Ok(collection)
}
