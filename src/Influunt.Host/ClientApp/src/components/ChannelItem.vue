<template>
    <b-list-group-item v-if="!deleted" class="bg-dark text-light" style="display:flex">
        <span style="width:85%; line-height: 32px;">{{ name }}</span>
        <div>
            <b-button variant="outline-warning" class="but" v-on:click="Edit"><b-icon icon="pencil" class="mr-2"/></b-button>
            <b-button variant="outline-warning" class="but" v-on:click="TurnChannelVisible"><b-icon v-bind:icon="channel.hidden ? 'eye-slash':'eye'" class="mr-2"/></b-button>
            <b-button variant="outline-danger" class="but" v-on:click="RemoveChannel"><b-icon icon="trash-fill" class="mr-2"/></b-button>
        </div>
    </b-list-group-item>
</template>
<script>
import InfluuntApi from "@/influunt"
export default {
    name:"ChannelItem",
    props:{
        name:String,
        id:String,
        url:String,
        channel:Object
    },
    data: function(){
        return {
            deleted:false
        }
    },
    methods:{
        Edit: function(){
            this.$emit("onEdit",this.channel)
        },
        TurnChannelVisible: function(){
            this.channel.hidden = !this.channel.hidden
            InfluuntApi.UpdateChannel(this.channel)
        },
        RemoveChannel: function(){
            var self = this
            InfluuntApi.RemoveChannel(self.id,function(r){
                // eslint-disable-next-line
                var rr = r
                self.$forceUpdate()
                self.deleted = true
            })
        }
    }
}
</script>

<style scoped>
    .but{
        width: 32px;
        height: 32px;
        padding: 5px;
        margin-right: 5px;
    }
    .list-group-item:hover{
        background-color: #333 !important;
    }
</style>