<template>
  <div
    id="app"
    class="bg-dark text-light"
    v-touch:swipe.right="swipeRight"
    v-touch:swipe.left="swipeLeft"
  >
    <TopMenu
      v-if="this.isLoaded"
      v-bind:isAuth="this.isAuthenticated"
      v-bind:email="this.profile.email"
    />
    <!--Main App-->
    <div v-if="this.isLoaded">
      <div v-if="this.isAuthenticated">
        <b-container fluid class="max-fluid-container-width">
          <b-row class="h-100vh">
            <b-col
              v-if="
                this.$store.state.application.isMenuOpened || !needMenuClosable
              "
              class="inrow-menu pt-4 pr-0"
              v-bind:class="[
                needMenuClosable ? 'ocmenu ocmenu-shadow bg-dark' : 'w-25',
              ]"
            >
              <b-list-group>
                <b-list-group-item to="/" class="light-item t-align-left">
                  <b-icon-rss class="mr-2" /> Feed
                </b-list-group-item>
                <b-list-group-item
                  to="/favorite"
                  class="light-item t-align-left"
                >
                  <b-icon-star-fill class="mr-2" /> Favorite
                </b-list-group-item>
                <b-list-group-item
                  to="/channels"
                  class="light-item t-align-left"
                >
                  <b-icon-receipt class="mr-2" /> Channels
                </b-list-group-item>
              </b-list-group>
              <div align="center">
                <span class="text-muted"
                  >Version: {{ this.$store.state.application.version }}</span
                >
              </div>
              <div align="center">
                <a class="text-muted" href="https://github.com/AMEST/influunt"
                  >Fork me on GitHub
                  <font-awesome-icon
                    :icon="{ prefix: 'fab', iconName: 'github' }"
                  />
                </a>
              </div>
            </b-col>

            <b-col class="work-shadow mw-75">
              <router-view />
            </b-col>
          </b-row>
        </b-container>
      </div>
      <!-- SingIn Page -->
      <div v-else>
        <Welcome />
      </div>
    </div>
  </div>
</template>

<script>
import { BIconRss, BIconStarFill, BIconReceipt } from "bootstrap-vue";
const TopMenu = () =>
  import(
    /* webpackChunkName: "topmenu-component" */ "@/components/TopMenu.vue"
  );
const Welcome = () =>
  import(/* webpackChunkName: "welcome-component" */ "@/views/Welcome.vue");
import InfluuntApi from "@/influunt";
export default {
  components: {
    TopMenu,
    Welcome,
    BIconRss,
    BIconStarFill,
    BIconReceipt,
  },
  data: function () {
    return {
      isAuthenticated: false,
      profile: { email: null },
      needMenuClosable: false,
      isLoaded: false,
      headerNavHeight: 58.31,
    };
  },
  methods: {
    // eslint-disable-next-line
    resizeHandler: function (e) {
      this.needMenuClosable = window.innerWidth <= 800 ? true : false;
    },
    swipeRight: function () {
      this.$store.commit("changeMenuState", true);
    },
    swipeLeft: function () {
      this.$store.commit("changeMenuState", false);
    },
    updateHeaderHeightVar: function () {
      var headerNav = window.document.getElementById("header-nav");
      if (!headerNav) {
        setTimeout(this.updateHeaderHeightVar, 300);
        return;
      }
      this.headerNavHeight =
        window.document.getElementById("header-nav").clientHeight;
      window.document
        .getElementById("app")
        .style.setProperty("--header-height", `${this.headerNavHeight}px`);
    },
  },
  created: function () {
    var self = this;
    InfluuntApi.CurrentUser(function (request) {
      var currentProfile = JSON.parse(request.response);
      if (currentProfile != null) {
        self.isAuthenticated = true;
        self.profile = currentProfile;
      }
      self.isLoaded = true;
    });
    InfluuntApi.GetVersion(function (request) {
      var versionResult = JSON.parse(request.response);
      self.$store.commit("changeVersion", versionResult.version);
    });
    this.needMenuClosable = window.innerWidth <= 800 ? true : false;
    window.addEventListener("resize", this.resizeHandler);
    window.addEventListener("resize", this.updateHeaderHeightVar);
    setTimeout(this.updateHeaderHeightVar, 300);
  },
};
</script>

<style>
body {
  background-color: #232221 !important;
}
body .bg-dark {
  background-color: #232221 !important;
}
body .bg-dark .card {
  background-color: #2f2f2f !important;
}
body .bg-dark .dropdown-menu {
  background-color: #232221 !important;
}
.dropdown-menu .dropdown-item:hover {
  background-color: #333 !important;
}
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  overflow: hidden;
  --header-height: 58.31px;
}

#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}
code {
  color: #e83e8c !important;
}
.inrow-menu .list-group .list-group-item {
  border-radius: 5px 0 0 5px;
  background-color: inherit;
  color: inherit;
  border: 0;
}
.light-item:hover,
.light-item:active,
.light-item-active {
  background-color: #333 !important;
}
.t-align-left {
  text-align: left !important;
}
.light-item a {
  width: 100% !important;
  height: 100% !important;
  display: block;
  color: #f8f9fa !important;
  text-align: left;
}
.light-item svg {
  text-align: left;
}
.inrow-menu {
  max-width: 25% !important;
}

.h-100vh {
  height: calc(100vh - var(--header-height)) !important;
}
.work-shadow {
  box-shadow: inset 0 0.5rem 1rem rgba(0, 0, 0, 0.35) !important;
}
.max-fluid-container-width {
  max-width: 1200px;
}
.mw-75 {
  min-width: 75% !important;
}

.ocmenu {
  max-width: 222px !important;
  position: fixed !important;
  z-index: 10;
  height: calc(100% - 56.6px);
}
.ocmenu-shadow {
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.35) !important;
}
.enable-scroll {
  overflow-y: auto !important;
  overflow-x: hidden;
}
/* Scroll */
/* width */
::-webkit-scrollbar {
  width: 10px;
}

/* Track */
::-webkit-scrollbar-track {
  background: transparent;
}

/* Handle */
::-webkit-scrollbar-thumb {
  background: #888;
}

/* Handle on hover */
::-webkit-scrollbar-thumb:hover {
  background: #555;
}
</style>
