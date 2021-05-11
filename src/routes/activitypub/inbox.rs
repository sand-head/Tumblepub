use actix_web::Responder;
use serde_json::json;
use tumblepub_ap::ActivityStreams;

use crate::errors::ServiceResult;

pub async fn get_ap_blog_inbox() -> ServiceResult<impl Responder> {
  // todo: implement getting inbox
  Ok(ActivityStreams(json!({})))
}

pub async fn post_ap_blog_inbox() -> ServiceResult<impl Responder> {
  // todo: implement posting inbox
  Ok(ActivityStreams(json!({})))
}
