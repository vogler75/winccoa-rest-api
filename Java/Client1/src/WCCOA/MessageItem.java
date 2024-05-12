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

import java.util.ArrayList;
import java.util.Dictionary;
import java.util.Map;

public class MessageItem {
    
    public Map<String, Object> data;

    public MessageItem(Map<String, Object> data) {
        this.data = data;
    }
    
    public MessageItem(Object data) {
        this.data = (Map)data;
    }

    public String toString() {
        return data != null ? data.toString() : "";
    }

    public String getFid() { 
        return HasKey("fid") ? data.get("fid").toString() : ""; 
    }

    public boolean HasKey(String key) {
        return (data != null && ((Map<String, Object>)data).containsKey(key));
    }

    public Integer getInt(String key) { 
        return HasKey(key) ? (Integer)data.get(key) : null; 
    }

    public String getString(String key) { 
        return HasKey(key) ? (String)data.get(key) : null; 
    }
    
    public ArrayList getArray(String key) {
        return HasKey(key) ? (ArrayList)data.get(key) : null; 
    }
    
    public Object getDynamic(String key) {
        return HasKey(key) ? (Object)data.get(key) : null;
    }    
    
    public MessageItem() {
        super();
    }
}
