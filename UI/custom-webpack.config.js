const webpack = require('webpack');

module.exports = {
    plugins: [
        new webpack.DefinePlugin({
            $ENV: {
                identityAPI: JSON.stringify(process.env.identityAPI),
                auctionAPI: JSON.stringify(process.env.auctionAPI),
                bidAPI: JSON.stringify(process.env.bidAPI),
                paymentAPI: JSON.stringify(process.env.paymentAPI),
                instrumentationKey: JSON.stringify(process.env.instrumentationKey)
            }
        })
    ]
};