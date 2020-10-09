CREATE TABLE blogs (
  id BIGSERIAL PRIMARY KEY,
  uri VARCHAR,
  name VARCHAR NOT NULL,
  domain VARCHAR,

  title VARCHAR,
  description TEXT,

  UNIQUE (name, domain)
);

CREATE TABLE users (
  id BIGSERIAL PRIMARY KEY,
  email VARCHAR UNIQUE NOT NULL,
  encrypted_password VARCHAR NOT NULL,
  primary_blog BIGINT REFERENCES blogs (id) NOT NULL
);

CREATE TABLE user_blogs (
  user_id BIGINT REFERENCES users (id) NOT NULL,
  blog_id BIGINT REFERENCES blogs (id) NOT NULL,

  is_admin BOOLEAN DEFAULT 'f',

  PRIMARY KEY (user_id, blog_id)
);