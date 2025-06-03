<template>
  <b-list-group-item
    v-if="!deleted"
    class="bg-dark text-light"
    style="display: flex"
  >
    <span style="width: 85%; line-height: 32px">{{ name }}</span>
    <div>
      <b-button variant="outline-warning" class="but" v-on:click="Edit"
        ><b-icon-pencil icon="pencil" />
      </b-button>
      <b-button
        variant="outline-warning"
        class="but"
        v-on:click="TurnChannelVisible"
      >
        <b-icon-eye-slash v-if="channel.hidden" class="mr-2" />
        <b-icon-eye v-else class="mr-2" />
      </b-button>
      <b-button variant="outline-danger" class="but" v-on:click="RemoveChannel">
        <b-icon-trash-fill icon="trash-fill" class="mr-2" />
      </b-button>
    </div>
  </b-list-group-item>
</template>
<script>
import {
  BIconPencil,
  BIconEye,
  BIconEyeSlash,
  BIconTrashFill,
} from "bootstrap-vue";
import InfluuntApi from "@/influunt";
export default {
  name: "ChannelItem",
  components: {
    BIconPencil,
    BIconEye,
    BIconEyeSlash,
    BIconTrashFill,
  },
  props: {
    name: String,
    id: String,
    url: String,
    channel: Object,
  },
  data: function () {
    return {
      deleted: false,
    };
  },
  methods: {
    Edit: function () {
      this.$emit("onEdit", this.channel);
    },
    TurnChannelVisible: function () {
      this.channel.hidden = !this.channel.hidden;
      InfluuntApi.UpdateChannel(this.channel);
    },
    RemoveChannel: function () {
      var self = this;
      InfluuntApi.RemoveChannel(self.id, function (r) {
        // eslint-disable-next-line
        var rr = r;
        self.$forceUpdate();
        self.deleted = true;
      });
    },
  },
};
</script>

<style scoped>
.but {
  width: 32px;
  height: 32px;
  padding: 5px;
  margin-right: 5px;
}
.list-group-item:hover {
  background-color: #333 !important;
}
.but svg{
    position: relative;
    top: -2px;
}
</style>