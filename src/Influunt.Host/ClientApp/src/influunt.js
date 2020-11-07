/*eslint-disable */
var InfluuntApi = {
    CurrentUser: function(callback = (request) => { }, async = true){
        var request = new XMLHttpRequest()
        request.open("GET", "/api/account/current", async)
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    GetFeed: function(callback = (request) => { }, offset = null, channel = null, async = true){
        var url = "/api/feed"

        if(channel != null)
            url += "/" + channel.id

        if(offset != null)
            url +="?offset="+offset

        var request = new XMLHttpRequest()
        request.open("GET", url, async)
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    GetChannels: function(callback = (request) => { }, async = true){
        var request = new XMLHttpRequest()
        request.open("GET", "/api/channel", async)
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    AddChannel: function(data, callback = (request) => { }, async = true){
        var request = new XMLHttpRequest()
        request.open("POST", "/api/channel", async)
        request.setRequestHeader("Content-Type","application/json")
        request.onload = function () {
            callback(request)
        }
        request.send(JSON.stringify(data))
    },
    RemoveChannel: function(id, callback = (request) => { }, async = true){
        var request = new XMLHttpRequest()
        request.open("DELETE", "/api/channel/"+id, async)
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    UpdateChannel: function(data, callback = (request) => { }, async = true){
        var request = new XMLHttpRequest()
        request.open("PUT", "/api/channel/"+data.id, async)
        request.setRequestHeader("Content-Type","application/json")
        request.onload = function () {
            callback(request)
        }
        request.send(JSON.stringify(data))
    },
    AddFavorite: function(data, callback = (request) => { }, async = true){
        var request = new XMLHttpRequest()
        request.open("POST", "/api/favorite", async)
        request.setRequestHeader("Content-Type","application/json")
        request.onload = function () {
            callback(request)
        }
        request.send(JSON.stringify(data))
    },
    GetFavorite: function(callback = (request) => { }, async = true){
        var request = new XMLHttpRequest()
        request.open("GET", "/api/favorite", async)
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    RemoveFavorite: function(id, callback = (request) => { }, async = true){
        var request = new XMLHttpRequest()
        request.open("DELETE", "/api/favorite/"+id, async)
        request.onload = function () {
            callback(request)
        }
        request.send()
    }
        
}
export default InfluuntApi
/* eslint-enable */
