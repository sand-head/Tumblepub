-- (temporarily) remove the current implementation of "group" blogs
-- originally, they were going to closely resemble how tumblr implements 
-- however, I have bigger ideas that necessitate re-implementation later down the line
-- therefore, there is now a many-to-one relationship of blogs to users, instead of many-to-many
-- see: https://github.com/sand-head/tumblepub/issues/17

DROP TABLE user_blogs;
ALTER TABLE blogs
  ADD COLUMN user_id BIGSERIAL REFERENCES users (id) NOT NULL,
  ADD COLUMN is_primary BOOLEAN NOT NULL DEFAULT false,
  -- why did I add both `is_public` *and* `is_private`
  DROP COLUMN is_private;
ALTER TABLE users
  DROP COLUMN primary_blog;

-- also, we're adding follows
-- technically, it is the primary blog that does the following, not the user
-- and since we don't know the details of remote "users",
-- follows are tracked on the blog level

CREATE TABLE follows (
  -- the blog that is following
  follower_id BIGINT REFERENCES blogs (id) NOT NULL,
  -- the blog that is being followed
  followee_id BIGINT REFERENCES blogs (id) NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  PRIMARY KEY (follower_id, followee_id)
);
