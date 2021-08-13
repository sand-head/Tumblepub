const { merge } = require("webpack-merge");
const common = require("./webpack.common.js");
const path = require("path");
const HtmlWebpackPlugin = require("html-webpack-plugin");

module.exports = merge(common, {
  mode: "development",
  devtool: "inline-source-map",
  devServer: {
    contentBase: "./dist",
  },
  plugins: [
    new HtmlWebpackPlugin({
      template: "./src/index.hbs",
      templateParameters: {
        Title: "justin",
        Description:
          "howdy! my name's Justin and this is a lil demo description for testing this theme<br><br>very cool!",
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
  ],
  module: {
    rules: [
      {
        test: /\.(hbs|handlebars)$/i,
        use: [
          {
            loader: "handlebars-loader",
            options: {
              debug: true,
              helperDirs: [path.resolve(__dirname, "helpers")],
              precompileOptions: {
                noEscape: true,
              },
            },
          },
        ],
      },
    ],
  },
});
