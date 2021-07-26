# `tumblepub`

[![CI](https://github.com/sand-head/tumblepub/actions/workflows/ci.yml/badge.svg)](https://github.com/sand-head/tumblepub/actions/workflows/ci.yml)

This directory houses the Tumblepub binary, which implements the GraphQL API and ActivityPub endpoints and renders blogs.

## Options

Options are read in both via environment variables (with the prefix `TUMBLEPUB_`) and an "options.yml" file. The following is an example of said file, using default values for each property:

````yaml
# Enables/disables "single user" mode
single_user_mode: true
# If `single_user_mode` is set to true, single_user options must also be set
# Example:
# ```
# single_user:
#   username: "bob"
#   email: "bob@example.com"
#   password: "password"
# ```
# By default, `single_user` is set to null to encourage adjusting the default account
single_user: null
# The domain name this instance exists on
local_domain: null
# PostgreSQL database properties
database:
  db_name: "tumblepub"
  hostname: "localhost"
  port: 5432
  username: "postgres"
  password: "mysecretpassword"
````
