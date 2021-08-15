use actix_web::{web, HttpResponse};
use identicon_rs::Identicon;

use crate::routes::BlogPath;

pub fn get_avatar_for_blog(path: web::Path<BlogPath>) -> HttpResponse {
  // todo: load avatars from some storage
  let default_icon = Identicon::new(path.blog.clone());
  HttpResponse::Ok()
    .content_type("image/png")
    .body(default_icon.export_png_data().unwrap())
}
