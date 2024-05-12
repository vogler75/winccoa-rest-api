//    Copyright (C) 2015 Andreas Vogler
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
var MessageBulk = function () {
    this.Vers = "";
    this.Type = "";
    this.Data = "";
}

var CallbackFunctions = [];
var FunctionIdentifier = 0;

var WCCOABase = function (host) {

    this.host = host;
    this.url = "http://" + host;
    this.cid = "";

    this.Test = function () {
        var a = ["PerfTest_1.Float"];
        var s = JSON.stringify(a, undefined);
        var u = this.url + "/Execute/dpGetValues?cid=" + encodeURIComponent(this.cid) + "&p=" + encodeURIComponent(s);
        var result = this.Request(u);
        console.log(result);
    }

    this.Request = function (url, data) {
        var deferred = $.Deferred();
        if (data == null) {
            var res = $.ajax({
                type: "GET",
                dataType: "json",
                async: true,
                context: this,
                url: url,
                success: function (data) {
                    deferred.resolve(this, data);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log("Request-Error", xhr.status);
                    console.log("Request-Error", thrownError);
                }
            });
            //return res.responseJSON;
        } else {
            var res = $.ajax({
                type: "POST",
                dataType: "json",
                async: true,
                context: this,
                data: data,
                url: url,
                success: function (data) {
                    deferred.resolve(this, data);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log("Request-Error", xhr.status);
                    console.log("Request-Error", thrownError);
                }
            });
            //return res.responseJSON;
        }
        return deferred.promise();
    }

    this.RequestPlain = function (url, data) {
        var xmlHttp = null;
        xmlHttp = new XMLHttpRequest();
        if (data != null) {
            xmlHttp.open("POST", url, false);
            xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        } else {
            xmlHttp.open("GET", url, false);
        }
        xmlHttp.send(data);
        if (xmlHttp.status != 200 /*OK*/)
            return null;
        else
            return JSON.parse(xmlHttp.responseText);
    }

    this.Open = function (callback) {
        $.support.cors = true;
        var u = this.url + "/Connect";
        $.when(this.Request(u)).done(function (sender, message) {
            if (message == null || message.Type != "C") return false;
            var data = JSON.parse(message.Data);
            if (data.length == 0) return false;
            sender.cid = data[0]["cid"];
            console.log("Connected to " + sender.url + " with id " + sender.cid);
            sender.Listen();
            if (callback != null)
                callback();
        });
    }

    this.Listen = function () {
        //$.ajax({ url: this.url + "/Stream?cid=" + encodeURI(this.cid), success: function (data) {
        //    console.log("stream: " + data);
        //}
        //});
        //console.log("Listen...done");

        $.when(this.Request(this.url + "/Stream?cid=" + this.cid)).done(function (sender, message) {
            if (message.Type == "M") {
                var items = JSON.parse(message.Data);
                for (i = 0; i < items.length; i++) {
                    var fid = items[i]["fid"];
                    console.log("listener got message", items[i]);
                    if (fid == "S") {
                        console.log("listener got script", items[i].script);
                        eval(items[i].script);
                    } else {
                        CallbackFunctions[fid](items[i]);
                    }
                }
            }
            sender.Listen();
        });

        /*
        var worker = new Worker('WCCOAListener.js');
        worker.addEventListener('message', function (e) {
        if (e.data.Type == "M") {
        //console.log('message ', e.data);
        document.getElementById('mydbg').value = e.data.Data;
        var items = JSON.parse(e.data.Data);
        for (i = 0; i < items.length; i++) {
        var fid = items[i]["fid"];
        console.log("listener got message", items[i]);
        CallbackFunctions[fid](items[i]);
        }
        }
        }, false);

        worker.postMessage({ url: this.url, cid: this.cid }); // Send data to our worker.
        */
    }



    this.dpGet = function (dps) {
        var a = (typeof dps == "string") ? [dps] : dps;
        var s = JSON.stringify(a, undefined);
        var u = this.url + "/Execute/dpGetValues?cid=" + encodeURIComponent(this.cid) + "&p=" + encodeURIComponent(s);
        var r = this.Request(u);
        var d = JSON.parse(r.Data);
        return d.hasOwnProperty("0") ? d["0"] : null;
    }

    this.evalScript = function (script) {
        var a = (typeof script == "string") ? (script) : null;
        var d = "cid=" + this.cid + "&f=" + encodeURIComponent(a);
        var u = this.url + "/Execute/evalScript";
        $.when(this.Request(u, d)).done(function (sender, message) {
            var r = JSON.parse(message.Data);
            return r.hasOwnProperty("0") ? d["0"] : null;
        });
    }

    this.dpConnectQuery = function (query, answer, callback) {
        var a = { query: query };
        var s = JSON.stringify(a, undefined);
        var f = FunctionIdentifier++;
        var u = this.url + "/Execute/dpQueryConnect?cid=" + encodeURIComponent(this.cid) + "&fid=" + encodeURIComponent(f) + "&a=" + (answer ? "1" : "0") + "&p=" + encodeURIComponent(s);
        console.log("dpConnectQuery", u);
        $.when(this.Request(u)).done(function (sender, message) {
            var d = JSON.parse(message.Data);
            CallbackFunctions[f] = callback;
            return d.hasOwnProperty("0") ? d["0"] : null;
        });
    }

};






    
