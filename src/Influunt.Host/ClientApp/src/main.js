import Vue from 'vue'
import App from './App.vue'
import router from './router'
import './registerServiceWorker'
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import Vue2TouchEvents from 'vue2-touch-events'
import store from './store'

import { library } from '@fortawesome/fontawesome-svg-core'
import { faGithub } from '@fortawesome/free-brands-svg-icons'
import { faUserSecret, faSync, faExclamationTriangle } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

// Add to home screen
window.addEventListener('beforeinstallprompt', function (event) {
  if ((window.localStorage.installPrompt === undefined) || (window.localStorage.installPrompt === 'undefined')) {
    window.localStorage.installPrompt = 'success'
    console.log('[beforeinstallprompt]', 'prompt')
    event.prompt()
  }
})
// Initialize FontAwesome
library.add(faGithub, faUserSecret, faSync, faExclamationTriangle)
Vue.component('font-awesome-icon', FontAwesomeIcon)
// Install BootstrapVue
Vue.use(BootstrapVue)
// Optionally install the BootstrapVue icon components plugin
Vue.use(IconsPlugin)
// Activate Vue2TouchEvents
Vue.use(Vue2TouchEvents)
Vue.config.productionTip = false

new Vue({
  store,
  router,
  render: h => h(App)
}).$mount('#app')
