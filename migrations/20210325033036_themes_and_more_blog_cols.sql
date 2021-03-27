-- probably should've added this extension sooner
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE blog_themes (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  -- still not sure if we want a hash of the theme, for comparison
  -- it'd probably make more sense in a nosql environment
  -- like should we index the hash? probably??
  hash VARCHAR NOT NULL,
  theme TEXT NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

ALTER TABLE blogs
  ADD COLUMN theme_id UUID REFERENCES blog_themes (id),
  ADD COLUMN is_nsfw BOOLEAN NOT NULL DEFAULT false,
  ADD COLUMN is_private BOOLEAN NOT NULL DEFAULT false;

-- also we'll just add a default value for posts too
ALTER TABLE posts
  ALTER COLUMN id SET DEFAULT uuid_generate_v4();