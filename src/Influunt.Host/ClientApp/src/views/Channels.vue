<template>
  <div class="Channels pt-2" style="text-align:left">
    <b-card class="light-gray-dark text-light mb-3" title="Add channel">
      <b-row>
        <b-col class="w-25" sm="3">
          <label>Name:</label>
        </b-col>
        <b-col class="w-75" sm="9">
          <b-form-input v-model="newChannelName"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="pt-2">
        <b-col  class="w-25" sm="3">
          <label>Url:</label>
        </b-col>
        <b-col class="w-75" sm="9">
          <b-form-input v-model="newChannelUrl"></b-form-input>
        </b-col>
      </b-row>
        <div align="right" class="mt-2">
          <b-button v-if="mode=='new'" variant="outline-secondary" v-on:click="AddChannel">Add channel</b-button>
          <b-button v-if="mode=='edit'" variant="outline-secondary" v-on:click="UpdateChannel">Update channel</b-button>
          <b-button variant="outline-secondary" class="ml-2" v-on:click="ClearState">Cancel</b-button>
        </div>
    </b-card>
    <b-list-group>
      <ChannelItem v-for="(item, key) in this.channels" 
        v-bind:key="key"
        v-bind:name="item.name" 
        v-bind:id="item.id" 
        v-bind:url="item.url"
        v-bind:channel=item
        @onEdit="OnEditChannel"
        />
    </b-list-group>
    <b-alert
      :show="this.alert.dismissCountDown"
      dismissible 
      v-bind:variant="this.alert.variant"
      @dismissed="this.alert.dismissCountDown=0"
      @dismiss-count-down="CountDownChanged"
      class="alert-position">
        {{ this.alert.text }}
      </b-alert>
  </div>
</template>
<script>
import ChannelItem from "@/components/ChannelItem.vue"
import InfluuntApi from "@/influunt"
export default {
  components:{
    ChannelItem
  },
  data: function(){
    return {
      channels:[],
      newChannelName:"",
      newChannelUrl:"",
      mode:"new",
      editChannel:{},
      alert:{
        text:"",
        dismissCountDown:0,
        variant:"danger"
      }
    }
  },
  methods:{
    AddChannel: function(){
      
      if(this.newChannelName == undefined || this.newChannelName == ""){
        this.ShowAlert("Channel name is empty",8)
        return
      }
    
      if(this.newChannelUrl == undefined || (!this.newChannelUrl.startsWith("http://") && !this.newChannelUrl.startsWith("https://"))){
        this.ShowAlert("Channel url expected start with http or https",8)
        return
      }

      var self = this
      InfluuntApi.AddChannel({
        "name":self.newChannelName,
        "url":self.newChannelUrl
      },
      // eslint-disable-next-line
      function(r){
        self.ClearState()
        setTimeout(function(){
          InfluuntApi.GetChannels(function(request){
            self.channels = JSON.parse(request.response)
            self.$forceUpdate()
          })
        },500)
      })

    },
    UpdateChannel: function(){
      if(this.newChannelName == undefined || this.newChannelName == ""){
        this.ShowAlert("Channel name is empty",8)
        return
      }
    
      if(this.newChannelUrl == undefined || (!this.newChannelUrl.startsWith("http://") && !this.newChannelUrl.startsWith("https://"))){
        this.ShowAlert("Channel url expected start with http or https",8)
        return
      }
      this.editChannel.name = this.newChannelName
      this.editChannel.url = this.newChannelUrl

      var self = this
      // eslint-disable-next-line
      InfluuntApi.UpdateChannel(this.editChannel, function(r){
        self.ClearState()
        setTimeout(function(){
          InfluuntApi.GetChannels(function(request){
            self.channels = JSON.parse(request.response)
            self.$forceUpdate()
          })
        },500)
      })

    },
    OnEditChannel: function(channel){
        this.mode = "edit"
        this.editChannel = channel
        this.newChannelName = channel.name
        this.newChannelUrl = channel.url
    },
    CountDownChanged: function(dismissCountDown) {
        this.alert.dismissCountDown = dismissCountDown
    },
    ShowAlert: function(text, seconds){
      this.alert.text = text
      this.alert.dismissCountDown = parseInt(seconds)
    },
    ClearState: function(){
      this.newChannelName = ""
      this.newChannelUrl = ""
      this.editChannel = {}
      this.mode = "new"
    }

  },
  mounted: function(){
    var self = this
    InfluuntApi.GetChannels(function(request){
      self.channels = JSON.parse(request.response)
    })
  }
}
</script>
<style scoped>
    .light-gray-dark{
        background-color: rgba(60, 67, 72) !important;
    }

    .form-control{
        background-color: transparent;
        color: #f8f9fa;
        border-color: #6c757d;
    }

    .alert-position{
      position: fixed;
      top: 64px;
      right: 0;
    }
</style>
