const path = require("path");

module.exports = {
    entry: "./Cork.Example.Fable.Http.fsproj",
    output: {
        path: path.join(__dirname, "./bin"),
        filename: "bundle.js",
    },
    mode: "development",
    target: "node",
    resolve: {
        // See https://github.com/fable-compiler/Fable/issues/1490
        symlinks: false,
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        }],
    },
};
