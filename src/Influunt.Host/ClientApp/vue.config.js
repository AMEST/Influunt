module.exports = {
    chainWebpack: (config) => {
        // Remove prefetch plugin and that's it!
        config.plugins.delete('prefetch')
    },
    pwa: {
        themeColor: '#ffc107',
        appleMobileWebAppStatusBarStyle: '#000000',
        workboxPluginMode: 'GenerateSW',
        workboxOptions: {
            runtimeCaching: [{
                urlPattern: new RegExp('(/api/version|/api/feed.*|/api/channel.*|/api/favorite.*|/api/account/current|/service\-worker\.js)'),
                handler: 'networkFirst',
                options: {
                    networkTimeoutSeconds: 30,
                    cacheName: 'api-cache',
                    cacheableResponse: {
                        statuses: [0, 200]
                    }
                }
            }]
        }
    }
}
