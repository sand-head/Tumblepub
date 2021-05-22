import { createHead } from '@vueuse/head';
import App from './App.vue';
import { routes } from './routes';
import viteSSR, { ClientOnly } from 'vite-ssr';

export default viteSSR(App, { routes }, ({ app }) => {
  const head = createHead();
  app.use(head);

  // add `ClientOnly` component
  app.component(ClientOnly.name, ClientOnly);

  return { head };
});
