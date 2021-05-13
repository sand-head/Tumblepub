use actix_web::{web, HttpResponse, Responder};
use handlebars::Handlebars;
use serde::Deserialize;
use sqlx::PgPool;
use tumblepub_db::models::blog::Blog;

use crate::{errors::ServiceResult, theme::ThemeVariables};

#[derive(Deserialize)]
pub struct BlogPath {
  pub blog: String,
}

pub async fn blog(
  path: web::Path<BlogPath>,
  pool: web::Data<PgPool>,
) -> ServiceResult<impl Responder> {
  let blog_name = &path.blog;
  let mut pool = pool.acquire().await.unwrap();

  let blog = Blog::find(&mut pool, (blog_name.clone(), None))
    .await
    .expect("Could not retrieve blog from database");
  if let None = blog {
    return Ok(HttpResponse::NotFound().finish());
  }
  let blog = blog.unwrap();

  let mut hbs = Handlebars::new();
  let default_theme = include_str!("../../themes/default.hbs");
  hbs
    .register_template_string("default", default_theme)
    .expect("Could not find default theme");

  let blog_title = blog.title.unwrap_or(blog.name);
  let vars = ThemeVariables {
    title: blog_title.clone(),
    description: blog.description,
    content: format!(
      "actually, there is no content for the blog \"{}\"",
      blog_title
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
