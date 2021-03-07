-- This file should undo anything in `up.sql`
ALTER TABLE users
ADD encrypted_password VARCHAR NOT NULL;

ALTER TABLE users
DROP hash;
ALTER TABLE users
DROP salt;