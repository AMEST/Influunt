<template>
    <div class="sync">
      <font-awesome-icon v-if="this.spin" style="color:#fff;" :icon="this.icon" class="mr-1" spin/>
      <font-awesome-icon v-else style="color:#fff;" :icon="this.icon" class="mr-1"/>
        {{this.text}}
    </div>
</template>

<script>
export default {
    name:"LoadingBar",
    props:{
        text:String,
        icon:String,
        spin:Boolean
    },
}
</script>

<style>
    .sync {
      width: 100%;
      max-height: 24px;
      position: fixed;
      background-color: rgba(255, 193, 7, 0.6);
      bottom: 0px;
      left: 0px;
      z-index: 9999;
      padding-left: 15px;
    }
</style>