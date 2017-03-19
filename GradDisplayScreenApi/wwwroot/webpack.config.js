const path = require('path');

module.exports = {
  context: __dirname + "/app",

  entry: {
      javascript: "./js/app.js"
  },

  output: {
    filename: "app.js",
    path: __dirname + "/dist",
  },

  resolve: {
    extensions: ['.js', '.jsx', '.json']
  },

  module: {
    loaders: [
      {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        loaders: ["babel-loader"]
      },
      {
          test: /\.html$/,
          loader: "file-loader?name=[name].[ext]",
      }
    ]
  }
}