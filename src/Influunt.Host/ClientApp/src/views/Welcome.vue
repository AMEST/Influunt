<template>
    <div class="row align-items-center h-100vh">
        <b-jumbotron class="bg-dark text-light width-max height-max jbg" header="Influunt Welcome">
            <p>Welcome to the lightweight RSS news feed aggregator.</p>
            <b-button variant="outline-danger" href="#" @click="auth">
                <img class="icon" src="../assets/google.png"/>
                Sing In with google
            </b-button>
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
          login: "/api/account/login/google"
      }
  },
  methods: {
      auth: function(){
          window.location.href = this.login
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