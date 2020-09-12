<template>
    <b-card class="bg-dark" v-bind:title="this.title" v-bind:sub-title="this.date + ' on ' + this.channel">
        <b-card-text>
            <span v-html="this.description"></span>
        </b-card-text>
        <b-button v-bind:href="this.link" target="_blank" variant="outline-secondary">Read more</b-button>
    </b-card>
</template>
<script>
export default {
    name:"FeedItem",
    props:{
        title:String,
        channel:String,
        description:String,
        date:String,
        link:String
    }
}
</script>

<style scoped>
    .card{
        margin-top: 15px;
        text-align: left;
        max-width: 99%;
    }
    .card-text{
        overflow-x: hidden;
    }
    .card:hover{
        background-color: rgba(60, 67, 72) !important;
    }
</style>