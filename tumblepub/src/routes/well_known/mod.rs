use actix_web::web;

mod nodeinfo;
mod webfinger;

pub fn routes(config: &mut web::ServiceConfig) {
  config.service(
    web::scope("/.well-known")
      .route("/nodeinfo", web::get().to(nodeinfo::nodeinfo))
      .route("/nodeinfo/2.1", web::get().to(nodeinfo::nodeinfo_2_1))
      .route("/webfinger", web::get().to(webfinger::webfinger)),
  );
}
