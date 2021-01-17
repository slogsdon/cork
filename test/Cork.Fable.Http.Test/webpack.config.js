const path = require("path");

module.exports = {
    mode: "development",
    entry: "./Cork.Fable.Http.Test.fsproj",
    output: {
        path: path.join(__dirname, "./bin"),
        filename: "tests.bundle.js",
    },
    target: "node",
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        }],
    },
};
