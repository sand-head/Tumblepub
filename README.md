# Tumblepub

[![Build (Core)](https://github.com/sand-head/tumblepub/actions/workflows/build-core.yml/badge.svg)](https://github.com/sand-head/tumblepub/actions/workflows/build-core.yml)
[![Docker Pulls](https://img.shields.io/docker/pulls/sandhead/tumblepub)](https://hub.docker.com/r/sandhead/tumblepub)

A federated tumbleblog hosting platform, for both individuals and communities. This repository contains two projects: the [core server](./core-server/README.md) and the web UI.

Tumblepub is under active development and any GraphQL queries/mutations and configuration options are subject to change during this time.

## Goals

(this is just off the top of my head right now, I'll add more to this later)

- [ ] Full S2S federation using the ActivityPub protocol
- [ ] A well-documented GraphQL API, enabling both web and mobile clients
- [ ] Theme support (using Handlebars as the templating engine)
  - (I also want to be able to give accessibility feedback to themes somehow...)
- [ ] Support for additional user-created pages
