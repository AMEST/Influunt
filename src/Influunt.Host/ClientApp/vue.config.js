module.exports = {
  pwa: {
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
