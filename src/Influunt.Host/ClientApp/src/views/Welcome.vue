<template>
    <div class="row align-items-center h-100vh">
        <b-jumbotron class="bg-dark text-light width-max height-max jbg" header="Influunt Welcome">
            <p>Welcome to the lightweight RSS news feed aggregator.</p>
            <b-button variant="outline-danger" href="/api/account/login/google">
                <img class="icon" src="../assets/google.png"/>
                Sing In with google
            </b-button>
        </b-jumbotron>
    </div>
</template>

<script>
export default {
  name: 'Welcome'
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
</style>