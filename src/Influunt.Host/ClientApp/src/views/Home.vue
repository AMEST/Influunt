<template>
  <div class="home pt-1 enable-scroll" id="feed">
    <b-card>
      <div>Filter by channel: <b-form-select v-model="selectedChannelFilter" :options="Array.from(channels, x => x.name)" class="bg-dark text-light" style="width: 220px"></b-form-select></div>
    </b-card>
    <br>
    <b-row v-if="this.feed.length == 0" class="h-max align-items-center">
      <b-col class="text-center">
        <b-icon-rss variant="warning" font-scale="7.5"/>
        <br>
        <span class="text-muted">News feed empty :(</span>
      </b-col>
    </b-row>
    <div v-for="(item, key) in this.feed" v-bind:key="key">
      <FeedItem v-bind:title="item.title" v-bind:description="item.description" v-bind:date="item.date" v-bind:link="item.link" v-bind:channel="item.channelName"/>
    </div>
    <LoadingBar v-if="this.isLoading" icon="sync" text="Loading feed" :spin="true"/>
    <ErrorBar v-if="this.isError" text="Error when loading feed" buttonText="Try reload feed" @onErrorButton="TryLoadAfterError"/>
  </div>
</template>

<script>
import InfluuntApi from "@/influunt"
import FeedItem from "@/components/FeedItem.vue"
import { BIconRss } from "bootstrap-vue";
const LoadingBar = () => import(/* webpackChunkName: "loading-bar-component" */"@/components/LoadingBar.vue");
const ErrorBar = () => import(/* webpackChunkName: "error-bar-component" */"@/components/ErrorBar.vue");
export default {
  name: 'home',
  components:{
    FeedItem,
    LoadingBar,
    ErrorBar,
    BIconRss
  },
  data: function(){
    return {
      feed:[],
      selectedChannelFilter:"All",
      channels:[{"name":"All"}],
      scrollMax: 0,
      offset:0,
      isLoading: false,
      isError: false
    }
  },
  methods:{
    InfinityFeed: function(offset){
      var self = this
      var channel = null
      if(this.selectedChannelFilter != "All"){
        channel = this.channels.find(c => c.name == this.selectedChannelFilter)
      }
      this.isLoading = true
      InfluuntApi.GetFeed(function(request, successful){
        if(!successful){
          self.isError = true
          self.isLoading = false
          return
        }

        var feedNews = JSON.parse(request.response)
        feedNews = feedNews.sort(function (a, b) {
          var dateA = new Date(a.date)
          var dateB = new Date(b.date)
          return dateA < dateB
        })
        var pushCounter = 0;
        feedNews.forEach(element => {
          if(!self.feed.some(x => x.itemHash === element.itemHash))
            self.feed.push(element)
        });
        if(feedNews.length > 2 && pushCounter < 2)
          self.scrollMax = self.scrollMax - 200

        self.isLoading = false
      },offset, channel)
    },
    TryLoadAfterError: function(){
      this.InfinityFeed(this.offset)
      this.isError = false
    },
    scrollTopViaBrand: function(){
        var feed = document.getElementById("feed")
        var brandElement = document.getElementById("brand")
        brandElement.onclick = function(){
            feed.scrollTo({
                top: 0,
                behavior: "smooth"
            });
        }
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
      this.isLoading = true
      InfluuntApi.GetFeed(function(request, successful){
        if(!successful){
          self.isError = true
          self.isLoading = false
          return
        }
        
        var feedNews = JSON.parse(request.response)
        feedNews = feedNews.sort(function (a, b) {
          var dateA = new Date(a.date)
          var dateB = new Date(b.date)
          return dateB - dateA
        })
        var pushCounter = 0;
        feedNews.forEach(element => {
          if(!self.feed.some(x => x.itemHash === element.itemHash))
            self.feed.push(element)
        });
        if(feedNews.length > 2 && pushCounter < 2)
          self.scrollMax = self.scrollMax - 200
        self.isLoading = false
      },this.offset, channel)
    }
  },
  mounted: function(){
    var self = this
    var feed = document.getElementById("feed")
    feed.onscroll = function(){   
        var headerNavHeight = window.document.getElementById("header-nav").clientHeight;
        var wh = window.innerHeight-headerNavHeight
        if ((feed.scrollTop + wh > feed.scrollHeight - wh / 2) && (self.scrollMax != feed.scrollHeight)) {
            // eslint-disable-next-line
            console.log("Download next 10 news. scrollTop:"+feed.scrollTop)
            self.scrollMax = feed.scrollHeight
            self.offset += 10
            self.InfinityFeed(self.offset)
        }
    }
    setTimeout(this.scrollTopViaBrand, 1000);
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