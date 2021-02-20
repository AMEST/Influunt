/*eslint-disable */
var InfluuntApi = {
    CurrentUser: function(callback = (request) => { }){
        var request = new XMLHttpRequest()
        request.open("GET", "/api/account/current")
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    GetFeed: function(callback = (request, successful) => { }, offset = null, channel = null){
        var url = "/api/feed"

        if(channel != null)
            url += "/" + channel.id

        if(offset != null)
            url +="?offset="+offset

        var request = new XMLHttpRequest()
        request.open("GET", url)
        request.onload = function () {
            callback(request, true)
        }
        request.onerror = function(){
            callback(request, false)
        }
        request.send()
    },
    GetChannels: function(callback = (request) => { }){
        var request = new XMLHttpRequest()
        request.open("GET", "/api/channel")
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    AddChannel: function(data, callback = (request) => { }){
        var request = new XMLHttpRequest()
        request.open("POST", "/api/channel")
        request.setRequestHeader("Content-Type","application/json")
        request.onload = function () {
            callback(request)
        }
        request.send(JSON.stringify(data))
    },
    RemoveChannel: function(id, callback = (request) => { }){
        var request = new XMLHttpRequest()
        request.open("DELETE", "/api/channel/"+id)
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    UpdateChannel: function(data, callback = (request) => { }){
        var request = new XMLHttpRequest()
        request.open("PUT", "/api/channel/"+data.id)
        request.setRequestHeader("Content-Type","application/json")
        request.onload = function () {
            callback(request)
        }
        request.send(JSON.stringify(data))
    },
    AddFavorite: function(data, callback = (request) => { }){
        var request = new XMLHttpRequest()
        request.open("POST", "/api/favorite")
        request.setRequestHeader("Content-Type","application/json")
        request.onload = function () {
            callback(request)
        }
        request.send(JSON.stringify(data))
    },
    GetFavorite: function(callback = (request) => { }, offset = null){
        console.log("[Offset]",offset)
        var url = "/api/favorite"

        if(offset != null)
            url +="?offset="+offset

        var request = new XMLHttpRequest()
        request.open("GET", url)
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    RemoveFavorite: function(id, callback = (request) => { }){
        var request = new XMLHttpRequest()
        request.open("DELETE", "/api/favorite/"+id)
        request.onload = function () {
            callback(request)
        }
        request.send()
    },
    GetVersion: function(callback = (request) => {}){
        var request = new XMLHttpRequest()
        request.open("GET", "/api/version")
        request.onload = function () {
            callback(request)
        }
        request.send()
    }
        
}
export default InfluuntApi
/* eslint-enable */
