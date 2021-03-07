-- Your SQL goes here
CREATE TABLE blogs (
  id BIGSERIAL PRIMARY KEY,
  uri VARCHAR,
  name VARCHAR NOT NULL,
  domain VARCHAR,
  is_public BOOLEAN NOT NULL DEFAULT 't',
  title VARCHAR,
  description TEXT,
  created_at TIMESTAMP NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMP NOT NULL DEFAULT NOW(),

  UNIQUE (name, domain)
);

CREATE TABLE posts (
  id UUID PRIMARY KEY,
  blog_id BIGINT REFERENCES blogs (id) NOT NULL
);

CREATE TABLE users (
  id BIGSERIAL PRIMARY KEY,
  email VARCHAR UNIQUE NOT NULL,
  primary_blog BIGINT REFERENCES blogs (id) NOT NULL,
  hash BYTEA NOT NULL,
  salt VARCHAR NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
  last_login_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE user_blogs (
  user_id BIGINT REFERENCES users (id) NOT NULL,
  blog_id BIGINT REFERENCES blogs (id) NOT NULL,
  is_admin BOOLEAN NOT NULL DEFAULT 'f',

  PRIMARY KEY (user_id, blog_id)
);