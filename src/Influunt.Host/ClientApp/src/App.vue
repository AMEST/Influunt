<template>
  <div id="app" class="bg-dark text-light">
    <TopMenu v-bind:isAuth="this.isAuthenticated" v-bind:email="this.profile.email"/>
    <!--Main App-->
    <div v-if="this.isAuthenticated">
      <b-container fluid class="max-fluid-container-width">
        <b-row class="h-100vh">

          <b-col class="inrow-menu w-25 pt-4 pr-0">
            <b-list-group>
              <b-list-group-item class="light-item">
                <router-link class="pl-1" to="/">
                  <b-icon icon="rss" class="mr-2"/> Feed
                </router-link>
              </b-list-group-item>
              <b-list-group-item class="light-item">
                <router-link to="/favorite">
                  <b-icon icon="star-fill" class="ml-1 mr-1"/> Favorite
                </router-link>
               </b-list-group-item>
              <b-list-group-item class="light-item">
                <router-link class="pl-1" to="/channels">
                  <b-icon icon="receipt" class="mr-2"/> Channels
                </router-link>
              </b-list-group-item>
            </b-list-group>
          </b-col>

          <b-col class="work-shadow mw-75">
            <router-view/>
          </b-col>

        </b-row>
      </b-container>
    </div>
    <!-- SingIn Page -->
    <div v-else>
      <Welcome/>
    </div>
  </div>
</template>

<script>
import TopMenu from "@/components/TopMenu.vue"
import Welcome from "@/views/Welcome.vue"
import InfluuntApi from "@/influunt"
export default {
  components:{
    TopMenu,
    Welcome
  },
  data: function(){
    return {
      isAuthenticated: false,
      profile: {email:null}
    }
  },
  created: function(){
    var self = this;
    InfluuntApi.CurrentUser(function(request){
      var currentProfile = JSON.parse(request.response)
      if(currentProfile != null){
        self.isAuthenticated = true
        self.profile = currentProfile
      }
    }, false)
  }
}
</script>

<style>
  #app {
    font-family: Avenir,Helvetica,Arial,sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
    overflow: hidden;
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
  code{
    color: #e83e8c!important;
  }
  .inrow-menu .list-group .list-group-item{
      border-radius: 5px 0 0 5px;
      background-color: inherit;
      color: inherit;
      border: 0;
  }
  .light-item:hover, .light-item:active, .light-item-active{
      background-color: rgba(60, 67, 72) !important;
  }
  .light-item a{
    width: 100%!important;
    height: 100%!important;
    display: block;
    color: #f8f9fa !important;
    text-align: left;
  }
  .inrow-menu{
        max-width: 25% !important;
    }

  .h-100vh{
      height: calc(100vh - 58.6px);
  }
  .work-shadow{
      box-shadow: inset 0 .5rem 1rem rgba(0,0,0,.35)!important;
  }
  .max-fluid-container-width{
    max-width: 1200px;
  }
  .mw-75 {
    min-width: 75%!important;
  }
  .enable-scroll{
    overflow-y: auto !important;
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
