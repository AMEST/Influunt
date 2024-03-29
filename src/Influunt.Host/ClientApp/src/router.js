import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
    routes: [
        {
          path: '/',
          name: 'home',
          component: Home
        },
        {
          path: '/favorite',
          name: 'favorite',
          component: () => import( /* webpackChunkName: "favorite" */ './views/Favorite.vue')
        },
        {
          path: '/channels',
          name: 'channels',
          component: () => import( /* webpackChunkName: "channels" */ './views/Channels.vue')
        },
        {
            path: '*',
            redirect: '/'
        }

    ]
})
