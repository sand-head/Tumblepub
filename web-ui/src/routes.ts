const pages = import.meta.glob('./pages/*.vue');

export const routes = Object.keys(pages).map((path) => {
  const name = path.match(/\.\/pages(.*)\.vue$/)![1].toLowerCase();
  return {
    path: name.toLowerCase().endsWith('/index') ? name.slice(0, -5) : name,
    component: pages[path],
  };
});
