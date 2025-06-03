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
    RemoveChannel: async function () {
      var confirmResult = await this.$bvModal.msgBoxConfirm(
        "Are you sure you want to delete this channel?",
        {
          title: "Confirm deletion",
          size: "sm",
          buttonSize: "sm",
          okVariant: "danger",
          okTitle: "Remove",
          cancelTitle: "Cancel",
          hideHeaderClose: false,
          centered: true,
          headerClass: "p-2 border-bottom-0",
          footerClass: "p-2 border-top-0",
          contentClass: "bg-dark text-light",
        }
      );
      if (!confirmResult) return;
      
      const self = this;
      // eslint-disable-next-line
      InfluuntApi.RemoveChannel(self.id, function (r) {
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
.but svg {
  position: relative;
  top: -2px;
}
</style>
