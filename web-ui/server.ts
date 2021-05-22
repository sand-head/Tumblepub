import fetch from 'node-fetch';
if (!globalThis.fetch) {
  (globalThis as any).fetch = fetch;
}

import path from 'path';
import Fastify, { FastifyInstance } from 'fastify';
import serveStatic from 'serve-static';
import { AddressInfo } from 'net';

// This contains a list of static routes (assets)
import { ssr } from './dist/server/package.json';
// The manifest is required for preloading assets
import manifest from './dist/client/ssr-manifest.json';
// This is the server renderer we just built
import { default as renderPage } from './dist/server/main';

// const api = require('./api');

(async () => {
  const fastify: FastifyInstance = Fastify();
  await fastify.register(require('middie'));

  // Serve every static asset route
  for (const asset of ssr.assets || []) {
    (fastify as any).use(
      '/' + asset,
      serveStatic(path.join(__dirname, `./dist/client/${asset}`))
    );
  }

  // Custom API to get data for each page
  // See src/main.js to see how this is called
  // api.forEach(({ route, handler, method = 'get' }) =>
  //   server[method](route, handler)
  // );

  // Everything else is treated as a "rendering request"
  fastify.get('/*', async (request, reply) => {
    const url = request.protocol + '://' + request.hostname + request.url;

    const { html } = await renderPage(url, {
      manifest,
      preload: true,
      // Anything passed here will be available in the main hook
      request,
      response: reply,
      // initialState: { ... } // <- This would also be available
    });

    reply.type('text/html').send(html);
  });

  try {
    await fastify.listen(3000);

    const address = fastify.server.address() as AddressInfo;
    console.log(`🚀 Listening on ${address.address}:${address.port}!`);
  } catch (e) {
    console.error(e);
    fastify.log.error(e);
    process.exit(1);
  }
})();
