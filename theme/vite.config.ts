import { UserConfig } from 'vite';
import handlebars from 'vite-plugin-handlebars';
import { createHtmlPlugin } from 'vite-plugin-html';
import { viteSingleFile } from 'vite-plugin-singlefile';
import helpers from './helpers';

const config: UserConfig = {
  plugins: [
    viteSingleFile(),
    createHtmlPlugin({
      minify: true,
    }),
    {
      ...handlebars({
        helpers,
        compileOptions: {
          noEscape: true,
        },
        context: {
          Title: "justin",
          Description:
            "howdy! my name's Justin and this is a lil demo description for testing this theme<br><br>very cool!",
          Avatar: "https://pbs.twimg.com/profile_images/1444764955505532929/1l5wrYIs_400x400.jpg",
          PreviousPage: null,
          NextPage: null,
          Posts: [
            {
              CreatedAt: "2021-07-28T22:35:58.196302+00:00",
              Content: [
                {
                  Type: "Markdown",
                  Content: "<h1>hello world!</h1>\n",
                },
              ],
            },
          ],
        },
      }),
      apply: 'serve',
    },
  ]
};

export default config;
