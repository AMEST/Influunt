<template>
  <div class="home pt-5 enable-scroll" id="feed">
    <div>Filter by channel: <b-form-select v-model="selectedChannelFilter" :options="Array.from(channels, x => x.name)" class="bg-dark text-light" style="width: 220px"></b-form-select></div>
    <b-row v-if="this.feed.length == 0" class="h-max align-items-center">
      <b-col class="text-center">
        <b-icon icon="rss" variant="warning" font-scale="7.5"/>
        <br>
        <span class="text-muted">News feed empty :(</span>
      </b-col>
    </b-row>
    <div v-for="(item, key) in this.feed" v-bind:key="key">
      <FeedItem v-bind:title="item.title" v-bind:description="item.description" v-bind:date="item.date" v-bind:link="item.link" v-bind:channel="item.channelName"/>
    </div>
  </div>
</template>

<script>
import InfluuntApi from "@/influunt"
import FeedItem from "@/components/FeedItem.vue"
export default {
  name: 'home',
  components:{
    FeedItem
  },
  data: function(){
    return {
      feed:[],
      selectedChannelFilter:"All",
      channels:[{"name":"All"}],
      scrollMax: 0,
      offset:0
    }
  },
  methods:{
    InfinityFeed: function(offset){
      var self = this
      var channel = null
      if(this.selectedChannelFilter != "All"){
        channel = this.channels.find(c => c.name == this.selectedChannelFilter)
      }

      InfluuntApi.GetFeed(function(request){
        var feedNews = JSON.parse(request.response)
        feedNews = feedNews.sort(function (a, b) {
          var dateA = new Date(a.date)
          var dateB = new Date(b.date)
          return dateA < dateB
        }).reverse()
        feedNews.forEach(element => {
          self.feed.push(element)
        });
      },offset, channel)
    }
  },
  watch:{
    selectedChannelFilter: function(val){
      var self = this
      var channel = null
      this.offset = 0
      this.scrollMax = 0
      this.feed = []
      if(val != "All"){
        channel = this.channels.find(c => c.name == val)
      }

      InfluuntApi.GetFeed(function(request){
        var feedNews = JSON.parse(request.response)
        feedNews = feedNews.sort(function (a, b) {
          var dateA = new Date(a.date)
          var dateB = new Date(b.date)
          return dateB - dateA
        })
        feedNews.forEach(element => {
          self.feed.push(element)
        });
      },this.offset, channel)
    }
  },
  mounted: function(){
    var self = this
    var feed = document.getElementById("feed")
    feed.onscroll = function(){   
      var wh = window.innerHeight-58.6
        if ((feed.scrollTop + wh > feed.scrollHeight - wh / 2) && (self.scrollMax != feed.scrollHeight)) {
          // eslint-disable-next-line
          console.log("Download next 10 news. scrollTop:"+feed.scrollTop)
          self.scrollMax = feed.scrollHeight
          self.offset += 10
          self.InfinityFeed(self.offset)
      }
    }
  },
  created: function(){
    var self = this
    this.InfinityFeed(0)
    InfluuntApi.GetChannels(function(request){
      let channels = JSON.parse(request.response)
      channels.forEach(element => {
        self.channels.push(element)
      })
    })
  }
}
</script>

<style scoped>
    .home{
      margin-top: 5px;
      max-height: calc(100vh - 63.8px);
      text-align: left;
    }
    .h-max{
      height: calc(100vh - 170px);
    }
</style>