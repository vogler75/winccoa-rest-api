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
import java.io.Console;
import java.io.IOException;

import java.io.InputStreamReader;

import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;

import java.util.Map;
import java.util.EventObject;
import java.util.EventListener;
import java.util.HashMap;


public class WCCOABase implements Runnable {
    protected String host = "";
    protected String url = "";
    protected String cid = "";
    protected int fid = 0;   
    
    private boolean isListening = false;
    
    private HashMap<String, ICallback> CallbackList = new HashMap<String, ICallback>();
    
    public void setHost(String host) {
        this.host = host;
    }

    public String getHost() {
        return host;
    }

    public void setUrl(String url) {
        this.url = url;
    }

    public String getUrl() {
        return url;
    }

    public void setCid(String cid) {
        this.cid = cid;
    }

    public String getCid() {
        return cid;
    }

    public String getFid() {
        return Integer.toString(++this.fid);
    }
        
    public WCCOABase(String host) {
        super();
        setHost(host);
        setUrl("http://"+host);        
    }
    
    public MessageBulk Request(String url) {
        try {
            URL iurl = new URL(url);
            HttpURLConnection request = (HttpURLConnection)iurl.openConnection();
            request.setRequestMethod("GET");
            
            int responseCode = request.getResponseCode();
            if ( responseCode != 200 ) return null;
            //System.out.println("responseCode: " + responseCode);
            
            BufferedReader reader = new BufferedReader(new InputStreamReader(request.getInputStream()));
            MessageBulk message = new MessageBulk(reader);
            reader.close();
            
            return message;
        } catch (IOException e) {
            System.out.println("Request-Exception: " +e.toString());
            return null;
        } 
    }
    
    public boolean Connect() {
        MessageBulk message = Request(getUrl() + "/Connect");
        
        if ( (message == null) || (message.getType().compareTo("C")!=0) ) return false;
        
        if ( message.Items().size() == 0 ) return false;
        
        cid = message.Items().get(0).getString("cid");
        
        System.out.println("Connected to " + this.url + " with id " + this.cid);
        
        (new Thread(this)).start();
        
        return true;
    }
    
    public String AddCallback(ICallback cb) {
        String fid = this.getFid();
        this.CallbackList.put(fid, cb);
        return fid;
    }
    
    public boolean RemoveCallback(String fid) {
        if ( this.CallbackList.containsKey(fid) ) {
            this.CallbackList.remove(fid);
            return true;
        } else {
            return false;            
        }
    }
    
    @Override
    public void run() {
        System.out.println("Listen to " + this.getHost() + " with id " + this.getCid());
        MessageBulk message;    
        
        try {
            this.isListening = true;
            while ( this.isListening ) {
                message = Request(this.getUrl() + "/Stream?cid=" + URLEncoder.encode(this.getCid(), "UTF-8"));
                if ( message != null && message.getType().compareTo("")!=0 ) {
                    // error
                    if (message.getType().compareTo("E")==0)
                    {
                      System.out.println("Listener error: " + message);
                      this.isListening = false;
                    }
                    
                    // message / function callback
                    else if (message.getType().compareTo("M")==0)
                    {                                           
                        for (MessageItem i : message.Items())
                          (this.CallbackList.get(i.getFid())).Callback(this, i);
                    }
                    else
                    {
                      System.out.println("Listener unknown type: " + message.getType());
                    }
                    
                }
            }
        } catch ( Exception e ) {
            System.out.println("Listener exception: "+e);
        }
    }
}
