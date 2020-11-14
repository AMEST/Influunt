import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    application: {
      isMenuOpened: false,
      version: ''
    }
  },
  getters: {},
  mutations: {
    changeMenuState (state, payload) {
      state.application.isMenuOpened = payload
    },
    changeVersion (state, value) {
      state.application.version = value
    }
  },
  actions: {}
})
