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
        Posts: [],
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
              helperDirs: [path.resolve(__dirname, "helpers")],
            },
          },
        ],
      },
    ],
  },
});
