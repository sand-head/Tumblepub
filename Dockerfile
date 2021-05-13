FROM rust:1.52 as builder
WORKDIR /usr/app
COPY . .

ENV SQLX_OFFLINE=true
RUN cargo build --release
RUN strip ./target/release/tumblepub

FROM debian:buster-slim as runtime
# make sure libssl is installed, for `openssl`
RUN apt-get update && \
    apt-get install libssl1.1 -y --no-install-recommends

COPY --from=builder /usr/app/target/release/tumblepub /usr/local/bin
EXPOSE 8080
ENTRYPOINT ["/usr/local/bin/tumblepub"]