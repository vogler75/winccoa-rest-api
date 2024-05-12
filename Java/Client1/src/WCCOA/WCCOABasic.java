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
package WCCOA;

import java.io.StringWriter;

import java.net.URI;
import java.net.URLEncoder;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

import org.json.simple.JSONArray;
import org.json.simple.JSONValue;

public class WCCOABasic extends WCCOABase {
    public WCCOABasic(String host) {
        super(host);
    }
    
    public MessageBulk dpGet(List<String> dps) {
        LinkedList list = new LinkedList();
        list.addAll(dps);
        StringWriter par = new StringWriter();
        try {
            JSONValue.writeJSONString(list, par);
            MessageBulk message =
                Request(this.getUrl() + "/Execute/dpGetValues"
                        + "?cid=" + URLEncoder.encode(this.getCid(), "UTF-8") 
                        + "&p=" + URLEncoder.encode(par.toString(), "UTF-8"));
            return message;
        } catch (Exception e) {
            System.out.println("dpGet exception: " + e);
        }
        return null;
    }
    
    public dpConnectValue dpConnect(ArrayList<String> dps, boolean answer) {
        return new dpConnectValue(this, dps, answer);
    }
    
    public dpConnectQuery dpConnectQuery(String query, boolean answer) {
        return new dpConnectQuery(this, query, answer);
    }  
}
