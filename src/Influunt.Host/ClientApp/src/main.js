import Vue from 'vue'
import App from './App.vue'
import router from './router'
import './registerServiceWorker'
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import Vue2TouchEvents from 'vue2-touch-events'
import store from './store'
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
