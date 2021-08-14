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
RUN cargo build --release --locked
RUN strip ./target/release/tumblepub

FROM debian:buster-slim as runtime
# make sure libssl is installed, for `openssl`
RUN apt-get update && \
    apt-get install libssl1.1 -y --no-install-recommends

WORKDIR /usr/app
RUN groupadd -g 999 tumblepub && \
    useradd -r -u 999 -g tumblepub tumblepub
COPY --chown=tumblepub:tumblepub --from=builder /usr/app/target/release/tumblepub .

USER tumblepub
EXPOSE 8080
ENTRYPOINT ["/usr/app/tumblepub"]