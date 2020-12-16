<template>
  <div class="favorite enable-scroll" id="feed">
    <b-row v-if="this.feed.length == 0" class="h-max align-items-center">
      <b-col class="text-center">
        <b-icon icon="star-fill" variant="warning" font-scale="7.5"/>
        <br>
        <span class="text-muted">Favorits empty :(</span>
      </b-col>
    </b-row>
    <div v-for="(item, key) in this.feed" v-bind:key="key">
      <FeedItem v-bind:title="item.title" v-bind:description="item.description" v-bind:date="item.date" v-bind:link="item.link" v-bind:id="item.id" v-bind:isFavorite="true" v-bind:channel="item.channelName"/>
    </div>
    <LoadingBar v-if="this.isLoading" icon="sync" text="Loading favorites" :spin="true"/>
  </div>
</template>

<script>
import InfluuntApi from "@/influunt"
import FeedItem from "@/components/FeedItem.vue"
import LoadingBar from "@/components/LoadingBar.vue"
export default {
name: 'favorite',
  components:{
    FeedItem,
    LoadingBar
  },
  data: function(){
    return {
      feed:[],
      isLoading: false
    }
  },
  created: function(){
    var self = this
    this.isLoading = true
    InfluuntApi.GetFavorite(function(request){
      var favoriteFeed = JSON.parse(request.response)
      favoriteFeed = favoriteFeed.sort(function (a, b) {
        var dateA = new Date(a.date)
        var dateB = new Date(b.date)
        return dateA < dateB
      }).reverse()
      favoriteFeed.forEach(element => {
        self.feed.push(element)
      });
      self.isLoading = false
    })
  }
}
</script>
<style scoped>
    .h-max{
      height: calc(100vh - 170px);
    }
    .favorite{
      margin-top: 5px;
      max-height: calc(100vh - 63.8px);
      text-align: left;
    }
</style>