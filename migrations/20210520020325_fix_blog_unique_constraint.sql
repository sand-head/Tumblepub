-- remove the bad constraint
ALTER TABLE blogs
  DROP CONSTRAINT blogs_name_domain_key;

-- add unique index, and also ensure domain is never ''
CREATE UNIQUE INDEX blogs_name_domain_idx ON blogs (name, COALESCE(domain, ''));
ALTER TABLE blogs
  ADD CONSTRAINT blogs_empty_domain_check CHECK (domain <> '');