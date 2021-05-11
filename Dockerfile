FROM rust:1.52 AS planner
WORKDIR /usr/app
# We only pay the installation cost once, 
# it will be cached from the second build onwards
RUN cargo install cargo-chef
COPY . .
RUN cargo chef prepare  --recipe-path recipe.json

FROM rust:1.52 AS cacher
WORKDIR /usr/app
RUN cargo install cargo-chef
COPY --from=planner /usr/app/recipe.json recipe.json
RUN cargo chef cook --release --recipe-path recipe.json

FROM rust:1.52 as builder
WORKDIR /usr/app
COPY . .
# Copy over the cached dependencies
COPY --from=cacher /usr/app/target target
COPY --from=cacher $CARGO_HOME $CARGO_HOME
ENV SQLX_OFFLINE=true
RUN cargo build --release

FROM rust:1.52 as runtime
WORKDIR /usr/app
COPY --from=builder /usr/app/target/release /usr/local/bin
ENTRYPOINT ["/usr/local/bin/tumblepub"]