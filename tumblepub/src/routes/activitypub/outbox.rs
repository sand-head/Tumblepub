use actix_web::Responder;
use serde_json::json;

use tumblepub_ap::activitypub_response;
use tumblepub_utils::errors::Result;

pub async fn get_ap_blog_outbox() -> Result<impl Responder> {
  // todo: implement getting outbox
  Ok(activitypub_response(&json!({})))
}

pub async fn post_ap_blog_outbox() -> Result<impl Responder> {
  // todo: implement posting outbox
  Ok(activitypub_response(&json!({})))
}
