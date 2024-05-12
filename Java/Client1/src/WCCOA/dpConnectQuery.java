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

import java.net.URLEncoder;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedList;

import org.json.simple.JSONValue;

public class dpConnectQuery extends dpConnectBase {
    
    private String query;
    
    public dpConnectQuery(WCCOABasic oa, String query, boolean answer) {        
        super(oa, answer);
        this.query = query;
        this.Connect();
    }


    @Override
    boolean ConnectImplementation() {
        System.out.println("dpConnectQuery");
        
        HashMap<String, String> list = new HashMap<String, String>();
        list.put("query", query);
        StringWriter par = new StringWriter();
        try {
            JSONValue.writeJSONString(list, par);
            //System.out.println("dpConnectValue: "+par.toString());
            MessageBulk message =
                oa.Request(oa.getUrl() + "/Execute/dpQueryConnect"
                        + "?cid=" + URLEncoder.encode(oa.getCid(), "UTF-8") 
                        + "&fid=" + URLEncoder.encode(this.fid, "UTF-8")
                        + "&a=" + (this.answer ? "1" : "0")
                        + "&p=" + URLEncoder.encode(par.toString(), "UTF-8"));
            return (message.Count()>0);
        } catch (Exception e) {
            System.out.println("dpConnectQuery exception: " + e);
        }
        return false;
    }

    @Override
    void DisconnectImplementation() {
        try {
            MessageBulk message =
                oa.Request(oa.getUrl() + "/Execute/dpQueryDisconnect"
                        + "?cid=" + URLEncoder.encode(oa.getCid(), "UTF-8") 
                        + "&fid=" + URLEncoder.encode(this.fid, "UTF-8"));
        } catch (Exception e) {
            System.out.println("dpGet exception: " + e);
        }    
    }    
    
}
