FROM rust:1.52 as builder
RUN apt-get update && apt-get install -y \
    software-properties-common \
    npm
RUN npm install npm@latest -g && \
    npm install n -g && \
    n lts
WORKDIR /usr/app
COPY . .

ENV SQLX_OFFLINE=true
RUN --mount=type=cache,target=/usr/local/cargo/git \
    --mount=type=cache,target=/usr/local/cargo/registry \
    --mount=type=cache,sharing=private,target=/usr/app/target \
    cargo build --release --locked && \
    cp ./target/release/tumblepub ./tumblepub
RUN strip ./tumblepub

FROM debian:buster-slim as runtime
# make sure libssl is installed, for `openssl`
RUN apt-get update && \
    apt-get install libssl1.1 -y --no-install-recommends

WORKDIR /usr/app
RUN groupadd -g 999 tumblepub && \
    useradd -r -u 999 -g tumblepub tumblepub
COPY --chown=tumblepub:tumblepub --from=builder /usr/app/tumblepub .

USER tumblepub
EXPOSE 8080
ENTRYPOINT ["/usr/app/tumblepub"]