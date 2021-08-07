use actix_web::{web, HttpResponse, Responder};

use tumblepub_ap::activity::{Activity, ActivityKind};
use tumblepub_utils::errors::Result;

pub async fn post_ap_blog_inbox(activity: web::Json<Activity>) -> Result<impl Responder> {
  match &activity.kind {
    ActivityKind::Create { object } => todo!(),
    ActivityKind::Announce => todo!(),
    ActivityKind::Follow => todo!(),
  }
  Ok(HttpResponse::Ok())
}
