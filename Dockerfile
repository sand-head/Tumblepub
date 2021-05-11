FROM rust:1.52 as builder
WORKDIR /usr/app
COPY . .
ENV SQLX_OFFLINE=true
RUN cargo build --release

FROM rust:1.52 as runtime
COPY --from=builder /usr/app/target/release /usr/local/bin
ENTRYPOINT ["/usr/local/bin/tumblepub"]