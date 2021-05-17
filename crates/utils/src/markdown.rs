use ammonia::clean;
use pulldown_cmark::{html::push_html, Options, Parser};

/// Uses `pulldown_cmark` and `ammonia` to convert Markdown into sanitized HTML.
pub fn markdown_to_safe_html<S: Into<String>>(markdown: S) -> String {
  let mut options = Options::empty();
  options.insert(Options::ENABLE_TABLES);
  options.insert(Options::ENABLE_STRIKETHROUGH);

  let markdown: String = markdown.into();
  let parser = Parser::new_ext(&markdown, options);
  let mut html = String::new();
  push_html(&mut html, parser);

  clean(&html)
}
