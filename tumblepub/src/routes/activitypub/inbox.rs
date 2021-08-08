use actix_web::{web, HttpResponse, Responder};

use serde_json::json;
use sqlx::PgPool;
use tumblepub_ap::{
  deliver, get_foreign_actor, get_name_from_uri,
  models::activity::{Activity, ActivityKind},
  verify_signature,
};
use tumblepub_db::models::blog::{Blog, NewBlog};
use tumblepub_utils::{
  crypto::Signature,
  errors::{Result, TumblepubError},
  options::Options,
};
use url::Url;

use super::BlogPath;

pub async fn post_ap_blog_inbox(
  activity: web::Json<Activity>,
  path: web::Path<BlogPath>,
  pool: web::Data<PgPool>,
  req: web::HttpRequest,
) -> Result<impl Responder> {
  let mut conn = pool.acquire().await.unwrap();
  // check if actor exists in db and, if not, acquire it
  let blog = if let Some(blog) = Blog::get_by_uri(&mut conn, &activity.actor).await? {
    blog
  } else {
    let actor_url =
      Url::parse(&activity.actor).map_err(|e| TumblepubError::InternalServerError(e.into()))?;
    let domain = actor_url.domain().unwrap();

    let actor = get_foreign_actor(&activity.actor).await?;
    Blog::create_new(
      &mut conn,
      NewBlog {
        user_id: None,
        uri: Some(actor.id),
        name: actor.preferred_username,
        domain: Some(domain.to_string()),
        title: Some(actor.name),
        description: actor.summary,
        is_primary: false,
        is_public: true,
        private_key: None,
        public_key: actor.public_key.public_key_pem.as_bytes().to_vec(),
      },
    )
    .await?
  };

  // obtain the signature from the headers and verify it
  let signature = Signature::parse(
    req
      .headers()
      .get("Signature")
      .and_then(|h| h.to_str().ok())
      .ok_or(TumblepubError::BadRequest("missing Signature header"))?,
  )?;
  if !verify_signature(signature, req.headers(), format!("/@{}/inbox", path.blog)).await? {
    return Err(TumblepubError::Unauthorized);
  }

  // finally, handle each activity type
  match &activity.kind {
    ActivityKind::Follow { object } => {
      let blog_name = get_name_from_uri(object)?;

      // add a follow to the database
      blog
        .follow_blog(&mut conn, (blog_name.clone(), None))
        .await?;

      // send an Accept activity back
      let local_domain = Options::get().local_domain;
      deliver(
        Activity {
          context: json!("https://www.w3.org/ns/activitystreams"),
          kind: ActivityKind::Accept,

          actor: format!("https://{}/@{}", local_domain, blog_name),
          to: vec![activity.actor.clone()],
          cc: vec![],
        },
        &blog.private_key.unwrap(),
      )
      .await?;

      Ok(HttpResponse::Created())
    }
    _ => todo!(),
  }
}
