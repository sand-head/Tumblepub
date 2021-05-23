FROM node:lts AS build
WORKDIR /usr/app
# Install yarn
RUN npm install -g -s --no-progress yarn

COPY ./package.json ../yarn.lock ./
RUN yarn install --frozen-lockfile

COPY . .
RUN yarn build

FROM node:lts AS runtime
WORKDIR /usr/app
# Install `ts-node` globally
RUN npm install -g -s --no-progress ts-node
COPY --from=builder /usr/app/dist ./dist
COPY --from=builder /usr/app/package.json /usr/app/tsconfig.json /usr/app/server.ts ./

ENTRYPOINT ["npm run start"]