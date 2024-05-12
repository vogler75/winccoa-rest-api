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

import java.io.BufferedReader;

import java.util.ArrayList;
import java.util.List;

import org.json.simple.*;

public class MessageBulk {
    private String Vers;
    private String Type;
    private String Data;    
    
    private ArrayList<MessageItem> ItemList;
    public int Count() {
        return Items().size();
    }
    
    public ArrayList<MessageItem> Items() {
        return ( ItemList == null ? Deserialize() : ItemList );
    }
                

    public String getVers() {
        return Vers;
    }

    public String getType() {
        return Type;
    }

    public String getData() {
        return Data;
    }

    
    public String toString() {
        return Vers + "/" + Type + "/" + Data;
    }
    
    public MessageBulk(String Vers, String Type, String Data) {        
        super();
        this.Vers = Vers;
        this.Type = Type;
        this.Data = Data;
    }
        
    public MessageBulk(BufferedReader reader) {
        Object obj = JSONValue.parse(reader);
        Vers = ((JSONObject) obj).get("Vers").toString();
        Type = ((JSONObject) obj).get("Type").toString();
        Data = ((JSONObject) obj).get("Data").toString();    
    }
    
    private ArrayList<MessageItem> Deserialize() {
        Object obj = JSONValue.parse(Data);
        JSONArray arr = (JSONArray)obj;
        ItemList = new ArrayList<MessageItem>();
        for ( int i=0; i<arr.size(); i++ ) {
            ItemList.add(new MessageItem(arr.get(i)));
        }
        return ItemList;
    }
}
