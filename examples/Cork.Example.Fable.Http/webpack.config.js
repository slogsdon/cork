const path = require("path");

module.exports = {
    mode: "development",
    entry: "./Cork.Example.Fable.Http.fsproj",
    output: {
        path: path.join(__dirname, "./bin"),
        filename: "bundle.js",
    },
    target: "node",
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        }],
    },
};
