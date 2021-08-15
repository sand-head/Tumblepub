use actix_web::web;

mod avatar;

pub fn routes(config: &mut web::ServiceConfig) {
  config.service(
    web::scope("/assets").route("/avatar/{blog}", web::get().to(avatar::get_avatar_for_blog)),
  );
}
