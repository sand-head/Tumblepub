use actix_web::{web, HttpResponse};
use serde_json::json;
use sqlx::PgPool;
use tumblepub_db::models::user::User;
use tumblepub_utils::{errors::Result, options::Options};

pub fn nodeinfo() -> HttpResponse {
  HttpResponse::Ok().json(json!([
    {
      "rel": "http://nodeinfo.diaspora.software/ns/schema/2.1",
      "href": format!("https://{}/.well-known/nodeinfo/2.1", Options::get().local_domain)
    }
  ]))
}

pub async fn nodeinfo_2_1(pool: web::Data<PgPool>) -> Result<HttpResponse> {
  let mut conn = pool.acquire().await.unwrap();
  let total_users = User::count(&mut conn).await?;

  Ok(HttpResponse::Ok().json(json!({
    "version": "2.1",
    "software": {
      "name": "Tumblepub",
      "version": "0.1.0",
    },
    "protocols": ["activitypub"],
    "services": {
      "inbound": [],
      "outbound": [],
    },
    "openRegistrations": !Options::get().single_user_mode,
    "usage": {
      "users": {
        "total": total_users,
      },
    },
    "metadata": {}
  })))
}
