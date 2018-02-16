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
    entry: resolve('./Cork.Fable.Http.Test.fsproj'),
    output: {
        filename: 'tests.bundle.js',
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