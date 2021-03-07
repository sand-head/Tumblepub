-- Your SQL goes here
ALTER TABLE users
DROP encrypted_password;

ALTER TABLE users
ADD hash BYTEA NOT NULL;
ALTER TABLE users
ADD salt VARCHAR NOT NULL;