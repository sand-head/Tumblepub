-- we need a table for all activitypub activities

CREATE TABLE activities (
  id UUID PRIMARY KEY,
  blog_id BIGINT REFERENCES blogs (id) NOT NULL,
  content JSONB NOT NULL
  is_local BOOLEAN NOT NULL DEFAULT true,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);