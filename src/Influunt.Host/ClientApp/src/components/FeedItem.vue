<template>
    <b-card v-if="!this.deleted" class="bg-dark" v-bind:title="this.title" v-bind:sub-title="this.date + ' on ' + this.channel">
        <b-card-text>
            <span v-html="this.description"></span>
        </b-card-text>
        <b-button v-bind:href="this.link" target="_blank" variant="outline-secondary">Read more</b-button>
        <b-button v-if="!this.inFavorite && !this.isFavorite" variant="outline-warning" class="favorite" @click="AddToFavorite"><b-icon icon="star-fill"/></b-button>
        <b-button v-if="this.isFavorite" variant="outline-warning" class="favorite" @click="RemoveFromFavorite"><b-icon icon="trash-fill"/></b-button>
    </b-card>
</template>
<script>
import InfluuntApi from "@/influunt"
export default {
    name:"FeedItem",
    props:{
        title:String,
        channel:String,
        description:String,
        date:String,
        link:String,
        id:String,
        isFavorite:Boolean
    },
    data: function(){
        return {
            inFavorite:false,
            deleted: false
        }
    },
    methods:{
        AddToFavorite: function(){
            var self = this
             /* eslint-disable */
            var item = {
                "title":this.title,
                "link":this.link,
                "channelName":this.channel,
                "description":this.description,
                "date":this.date
            }
            InfluuntApi.AddFavorite(item, function(request){
                self.inFavorite = true
            })
             /* eslint-enable */
        },
        RemoveFromFavorite: function(){
            var self = this
            // eslint-disable-next-line
            InfluuntApi.RemoveFavorite(this.id, function(request){
                self.deleted = true
            })
        }
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
    .favorite{
        display: inline-block;
        right: 15px;
        position: absolute;
    }
</style>