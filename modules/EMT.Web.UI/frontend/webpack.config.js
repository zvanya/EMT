const { resolve } = require('path')
const HtmlWebpackPlugin = require('html-webpack-plugin')
const webpack = require('webpack')
const config = require('./config.json')

module.exports = env => {
    return {
        'entry': './index.js',
        'output': {
            'filename': 'bundle.js',
            'path': resolve(__dirname, env.prod ? '../assets' : 'public'),
            'pathinfo': !env.prod,
            'publicPath': '/assets'
        },
        'context': resolve(__dirname, 'src'),
        'devtool': env.prod ? 'source-map' : 'eval',
        'module': {
            'loaders': [
                { 'test': /\.js/, 'loader': 'babel-loader', 'exclude': /node_modules/ },
                { 'test': /\.scss/, 'loader': 'style-loader!css-loader!sass-loader' },
                { 'test': /\.css/, 'loader': 'style-loader!css-loader' }
            ]
        },
        'externals': {
            'jquery': 'jQuery',
            '$': 'jQuery',
            'CanvasJS': 'CanvasJS',
            'd3': 'd3',
            'Rickshaw': 'Rickshaw',
            'Papa': 'Papa'
        },
        'plugins': [
            new HtmlWebpackPlugin({
                'template': './index.template.html',
                'hash': true,
                'filename': env.prod ? '../pages/index.html' : 'index.html'
            }),
            new webpack.DefinePlugin({
                API_HOST: JSON.stringify(config.api_host)
, HOST: JSON.stringify(config.host)  }),
            new webpack.ProvidePlugin({
                'Router': 'react-router',
                'nv': 'nvd3',
                'React': 'react',
                'ReactDOM': 'react-dom'
            }),
            new webpack.NoEmitOnErrorsPlugin()
        ],
        'devServer': {
            'historyApiFallback': true
        }
    }
}
