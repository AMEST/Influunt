<template>
    <b-list-group-item v-if="!deleted" class="bg-dark text-light" style="display:flex">
        <span style="width:100%; line-height: 32px;">{{ name }}</span>
        <div>
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
        url:String
    },
    data: function(){
        return {
            deleted:false
        }
    },
    methods:{
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
    }
    .list-group-item:hover{
        background-color: rgba(60, 67, 72) !important;
    }
</style>