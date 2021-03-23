use actix_web::web;

mod webfinger;

pub fn routes(config: &mut web::ServiceConfig) {
  config
    .service(web::scope("/.well-known").route("/webfinger", web::get().to(webfinger::webfinger)));
}
