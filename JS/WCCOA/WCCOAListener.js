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
self.addEventListener('message', function (e) {
    console.log("start listener " + e.data.url + " / " + e.data.cid);
    while (true) {
        xmlHttp = new XMLHttpRequest();
        xmlHttp.open("GET", e.data.url + "/Stream?cid=" + encodeURI(e.data.cid), false);
        xmlHttp.send(null);
        if (xmlHttp.status == 200 /*OK*/) {
            self.postMessage(JSON.parse(xmlHttp.responseText));
        }
    }
}, false);


