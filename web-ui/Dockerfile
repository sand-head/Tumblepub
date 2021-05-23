FROM node:lts AS build
WORKDIR /usr/app

COPY ./package.json ./yarn.lock ./
COPY ./web-ui/package.json ./web-ui/
RUN yarn install --frozen-lockfile

WORKDIR /usr/app/web-ui
COPY ./web-ui ./
RUN yarn build

FROM node:lts AS runtime
WORKDIR /usr/app
COPY --from=build /usr/app/web-ui/dist ./dist
COPY --from=build /usr/app/web-ui/package.json /usr/app/web-ui/tsconfig.json /usr/app/web-ui/server.js ./
COPY --from=build /usr/app/yarn.lock ./

ENV NODE_ENV=production
RUN yarn install --frozen-lockfile --prod

EXPOSE 3000
ENTRYPOINT ["node", "server"]