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

import java.io.IOException;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class Client1 {

    public static void main(String[] args) {
        WCCOABasic oa = new WCCOABasic("localhost:8080");
        MessageBulk message = null;
        
        oa.Connect();
        for ( int i=1; i<=1; i++ ) {
            message = oa.dpGet(new ArrayList<String>(Arrays.asList("ExampleDP_Rpt1.")));
            System.out.print("\r"+message);
        }
        if ( message != null ) System.out.println(message);
        
        //dpConnectValue c = oa.dpConnect(new ArrayList<String>(Arrays.asList("PerfTest_1.Float", "PerfTest_2.Float", "PerfTest_3.Float")), true);
        dpConnectQuery c = oa.dpConnectQuery("SELECT '_original.._value', '_original.._stime' FROM '*.**'", false);

        try {
            System.in.read();
        } catch (IOException e) {
        }
    }
}
