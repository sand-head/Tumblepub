import { renderer } from '@bikeshaving/crank/dom';
import { createElement } from '@bikeshaving/crank';
import App from './App';

(async () => {
  await renderer.render(<App />, document.getElementById('root')!);
})();

if (import.meta.hot) {
  import.meta.hot.accept();
}