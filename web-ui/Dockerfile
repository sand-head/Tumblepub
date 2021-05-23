FROM node:lts AS build
WORKDIR /usr/app
# Install yarn
# RUN npm install -g --no-progress yarn

COPY ./package.json ./web-ui/package.json ./yarn.lock ./
RUN yarn install --frozen-lockfile

COPY ./web-ui ./web-ui
RUN cd ./web-ui && yarn build

FROM node:lts AS runtime
WORKDIR /usr/app
COPY --from=build /usr/app/web-ui/dist ./dist
COPY --from=build /usr/app/web-ui/package.json /usr/app/web-ui/tsconfig.json /usr/app/web-ui/server.js ./
COPY --from=build /usr/app/yarn.lock ./

RUN yarn install --frozen-lockfile --prod

EXPOSE 3000
ENTRYPOINT ["node", "server"]