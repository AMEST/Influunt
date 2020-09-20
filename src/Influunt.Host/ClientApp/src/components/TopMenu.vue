<template>
    <div>
        <b-navbar toggleable="lg" type="dark" variant="dark" class="shadow">
            <b-container>
                <b-icon v-if="needMenuClosable" icon="list" class="menu-button" @click="changeMenuState" />
                <b-navbar-brand href="#">
                    <img class="topicon" src="../assets/rss.png" />Influunt
                </b-navbar-brand>
                <!-- Right aligned nav items -->
                <b-navbar-nav class="ml-auto">
                    <b-nav-item-dropdown v-if="this.isAuth" right>
                        <!-- Using 'button-content' slot -->
                        <template v-slot:button-content>
                            <em>{{email}}</em>
                        </template>
                        <b-dropdown-item href="/api/account/SignOut">Sign Out</b-dropdown-item>
                    </b-nav-item-dropdown>
                </b-navbar-nav>
            </b-container>
        </b-navbar>
    </div>
</template>

<script>
export default {
  name: 'TopMenu',
  props:{
      isAuth: Boolean,
      email: String
  },
  data: function(){
    return {
      needMenuClosable: false
    }
  },
  methods:{
    changeMenuState: function(){
          this.$store.commit("changeMenuState", !this.$store.state.application.isMenuOpened);
      },
    // eslint-disable-next-line
    resizeHandler: function(e){
      this.needMenuClosable = (window.innerWidth <= 800)? true: false
    }
  },
  created: function(){
    this.needMenuClosable = (window.innerWidth <= 800)? true: false
    window.addEventListener("resize", this.resizeHandler);
  }
}
</script>

<style>
    .topicon{
        padding-bottom: 5px;
        padding-right: 5px;
        max-width: 32px;
    }
    .menu-button{
        margin-right: 15px;
        width: 32px;
        height: 32px;
        cursor: pointer;
    }
</style>