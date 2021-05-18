# Tumblepub

[![Build](https://github.com/sand-head/tumblepub/actions/workflows/build.yml/badge.svg)](https://github.com/sand-head/tumblepub/actions/workflows/build.yml)
[![Docker Pulls](https://img.shields.io/docker/pulls/sandhead/tumblepub)](https://hub.docker.com/r/sandhead/tumblepub)

A federated tumbleblog hosting platform, for both individuals and communities.

Tumblepub is under active development and any GraphQL queries/mutations and configuration options are subject to change during this time.

## Goals

(this is just off the top of my head right now, I'll add more to this later)

- [ ] Full S2S federation using the ActivityPub protocol
- [ ] A well-documented GraphQL API, enabling both web and mobile clients
- [ ] Theme support (using Handlebars as the templating engine)
  - (I also want to be able to give accessibility feedback to themes somehow...)
- [ ] Support for additional user-created pages

## Options

Options are read in both via environment variables (with the prefix `TUMBLEPUB_`) and an "options.yml" file. The following is an example of said file, using default values for each property:

```yaml
# Enables/disables "single user" mode
single_user_mode: true
# The domain name this instance exists on
local_domain: null
# PostgreSQL database properties
database:
  db_name: "tumblepub"
  hostname: "localhost"
  port: 5432
  username: "postgres"
  password: "mysecretpassword"
```
