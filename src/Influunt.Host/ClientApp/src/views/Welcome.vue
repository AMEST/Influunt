<template>
    <div class="row align-items-center h-100vh">
        <b-jumbotron class="bg-dark text-light width-max height-max jbg" header="Influunt Welcome">
            <h5 class="pb-4">Welcome to the lightweight RSS news feed aggregator.</h5>
            <b-button variant="outline-danger" href="#" class="auth-standart-button-size" @click="auth">
                <img class="icon" src="../assets/google.png"/>
                Sing In with google
            </b-button>
            <p class="pt-2"> Or </p>
            <b-button variant="outline-danger" href="#" class="auth-standart-button-size" @click="guestAuth">
                <font-awesome-icon style="color:#fff;" size="lg" icon="user-secret" class="mr-2" />
                Sing In as guest
            </b-button>
            <p>Try Influunt as a guest. All data is visible to other guests!</p>
        </b-jumbotron>
        <div align="center" class="fork-me"> 
              <a class="text-muted" href="https://github.com/AMEST/influunt" >Fork me on GitHub <font-awesome-icon :icon="{ prefix: 'fab', iconName: 'github' }" /> </a>
        </div>
    </div>
</template>

<script>
export default {
  name: 'Welcome',
  data: function(){
      return {
          login: "/api/account/login/google",
          guestLogin:"/api/account/login/guest"
      }
  },
  methods: {
      auth: function(){
          window.location.href = this.login
      },
      guestAuth: function(){
          window.location.href = this.guestLogin
      }
  }
}
</script>

<style scoped>
    .h-100vh{
        height: calc(100vh - 58.8px);
    }
    .width-max{
        width: 100%;
    }
    .height-max{
        height: 100%;
    }
    .icon{
        padding-right: 5px;
        max-width: 32px;
    }
    .auth-standart-button-size{
        width: 200px;
    }
    .jbg{
        background-image: url("/img/bg.jpg");
        background-position: center;
    }
    .fork-me{
        position: fixed;
        bottom: 0px;
        left: 25px;
    }
</style>