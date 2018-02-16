const path = require('path');
const fs = require('fs');

const resolve = (filePath) => path.resolve(__dirname, filePath);

const babelOptions = {
    "presets": [
        [resolve("../../node_modules/babel-preset-env"), {
            "modules": false
        }]
    ]
}

module.exports = {
    entry: resolve('./Spile.Example.Fable.Http.fsproj'),
    output: {
        filename: 'bundle.js',
        path: resolve('./bin'),
    },
    target: "node",
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: {
                loader: "fable-loader",
                options: {
                    babel: babelOptions
                }
            }
        }, {
            test: /\.js$/,
            exclude: /node_modules\/(?!fable)/,
            use: {
                loader: 'babel-loader',
                options: babelOptions
            },
        }]
    },
};