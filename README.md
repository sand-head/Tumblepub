# tumblepub

[![Build](https://github.com/sand-head/tumblepub/actions/workflows/build.yml/badge.svg)](https://github.com/sand-head/tumblepub/actions/workflows/build.yml)
[![Docker Pulls](https://img.shields.io/docker/pulls/sandhead/tumblepub)](https://hub.docker.com/r/sandhead/tumblepub)

an ActivityPub-federated tumbleblog hosting platform

## some goals

this is just off the top of my head right now, I'll add more to this later:

- [ ] Full federation using ActivityPub and ActivityStreams
- [ ] A well-documented GraphQL API, enabling both web and mobile clients
- [ ] Theme support (using Handlebars as the templating engine)
- [ ] Support for additional user-created pages

## options

options are read in both via environment variables (with the prefix `TUMBLEPUB_`) and an "options.yml" file. the following is an example of said file, using default values for each property.

```yaml
# Enables/disables "single user" mode
single_user_mode: true
# The domain name this instance exists on
local_domain: null
# PostgreSQL database properties
database:
  database: "tumblepub"
  hostname: "localhost"
  port: 5432
  username: "postgres"
  password: "mysecretpassword"
```
