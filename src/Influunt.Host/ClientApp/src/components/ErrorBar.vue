<template>
    <div class="error">
        <font-awesome-icon style="color:#fff;" icon="exclamation-triangle" class="mr-1"/>
        {{this.text}}
        <b-button v-if="this.buttonText != undefined" variant="outline-warning" @click="Callback">{{this.buttonText}}</b-button>
    </div>
</template>

<script>
export default {
    name:"ErrorBar",
    props:{
        text:String,
        buttonText:String,
        hasButton:Boolean
    },
    methods:{
        Callback: function(){
            this.$emit("onErrorButton")
        }
    }
}
</script>

<style>
    .error {
        width: 100%;
        max-height: 24px;
        position: fixed;
        background-color: #e74c3c;
        bottom: 0px;
        left: 0px;
        z-index: 9999;
        padding-left: 15px;
    }
    .error button{
        max-height: 21px;
        font-size: 13px;
        padding: 0;
        margin-left: 8px;
    }
</style>