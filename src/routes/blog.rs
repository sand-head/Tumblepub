use actix_web::{web, HttpResponse, Responder};
use handlebars::Handlebars;
use serde::Deserialize;

use crate::{errors::ServiceResult, theme::ThemeVariables};

#[derive(Deserialize)]
pub struct BlogPath {
  pub blog: String,
}

pub async fn blog(path: web::Path<BlogPath>) -> ServiceResult<impl Responder> {
  let mut hbs = Handlebars::new();
  hbs
    .register_template_file("default", "./themes/default.hbs")
    .expect("Could not find default theme");

  let vars = ThemeVariables {
    title: "tumblepub".to_string(),
    description: None,
    content: format!(
      "actually, there is no content for the blog \"{}\"",
      path.blog
    )
    .to_string(),

    previous_page: None,
    next_page: None,
  };
  Ok(
    HttpResponse::Ok().body(
      hbs
        .render("default", &vars)
        .expect("Could not render theme"),
    ),
  )
}

pub fn routes(config: &mut web::ServiceConfig) {
  config.service(web::resource("/@{blog}").route(web::get().to(blog)));
}
