<template>
    <b-card v-if="!this.deleted" v-bind:title="this.title" v-bind:sub-title="this.date + ' on ' + this.channel">
        <b-card-text>
            <span v-html="this.description"></span>
        </b-card-text>
        <div style="min-height: 46px">
            <b-button v-if="this.link" v-bind:href="this.link" target="_blank" variant="outline-secondary">Read more</b-button>
            <b-button v-if="!this.inFavorite && !this.isFavorite" variant="outline-warning" class="favorite-button" @click="AddToFavorite"><b-icon-star-fill class="favorite-icon"/></b-button>
            <b-button v-if="this.isFavorite" variant="outline-warning" class="favorite-button" @click="RemoveFromFavorite"><b-icon-trash-fill/></b-button>
        </div>
    </b-card>
</template>
<script>
import {BIconStarFill, BIconTrashFill} from "bootstrap-vue";
import InfluuntApi from "@/influunt"
export default {
    name:"FeedItem",
    components: {
        BIconStarFill, BIconTrashFill
    },
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

<style>
    .card{
        text-align: left;
        max-width: 99%;
    }
    .card-text{
        overflow-x: hidden;
    }
    .card-text img{
        width: 100%!important;
    }
    .card:hover{
        background-color: #333 !important;
    }
    .favorite-button{
        display: inline-block;
        right: 15px;
        position: absolute;
    }
    .favorite-icon {
        margin-bottom: 2px;
    }
</style>