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

public abstract class dpConnectBase implements ICallback {
    
    protected WCCOABasic oa;
    protected String fid;
    protected boolean answer;
    
    private boolean connected;
    
    private static int counter=0;
   
    public dpConnectBase(WCCOABasic oa, boolean answer) {
        super();       
        this.oa = oa;
        this.answer = answer;
        this.fid = oa.AddCallback(this);
    }

    abstract boolean ConnectImplementation();
    
    public void Connect() {
        System.out.println("dpConnectBase");
        this.connected = ConnectImplementation();
    }
    
    abstract void DisconnectImplementation();
    
    public void Disconnect() {
        if ( connected ) {
            DisconnectImplementation();
            connected = false;
        }
    }
    
    public void finalize() {
        this.Disconnect();
    }

    @Override
    public void Callback(Object sender, MessageItem message) {
        //if ( ++counter % 100 == 0 ) System.out.print("\rCallback " + counter + ": " +message);
        System.out.println(message);
    }
}
