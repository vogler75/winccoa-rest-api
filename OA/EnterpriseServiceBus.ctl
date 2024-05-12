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
unsigned g_id=0;
mapping g_messages;
mapping g_clients;
mapping g_connects;
int g_message_counter=0;

#uses "CtrlHTTP"
#uses "CtrlXmlRpc"

main()
{
  if (httpServer(false, 8080, 0) < 0)
  {
   //DebugN("ERROR: HTTP-server can't start. --- Check license");
   return;
  }
  
  httpConnect("cbConnect", "/Connect");
  httpConnect("cbStream",  "/Stream");
  
  httpConnect("cbExecute_evalScript",           "/Execute/evalScript");
  httpConnect("cbExecute_dpNames",              "/Execute/dpNames");    
  httpConnect("cbExecute_dpGetValues",          "/Execute/dpGetValues");  
  httpConnect("cbExecute_dpGetMapping",         "/Execute/dpGetMapping");    
  httpConnect("cbExecute_dpSet",                "/Execute/dpSet");    
  httpConnect("cbExecute_dpConnect",            "/Execute/dpConnect");    
  httpConnect("cbExecute_dpDisconnect",         "/Execute/dpDisconnect");      
  httpConnect("cbExecute_dpQueryConnect",       "/Execute/dpQueryConnect");  
  httpConnect("cbExecute_dpQueryDisconnect",    "/Execute/dpQueryDisconnect");      

  startThread("checkClients");
  
  // test  
  /*
  dyn_string dps = dpNames("PerfTest_*", "PerfTest");
  for ( int i=1; i<=dynlen(dps) && i<=100; i++ ) 
    dps[i]+=".Float";
  
  DebugTN(dynlen(dps));
  dpConnect("cbTestMessage", false, dps);  
  */
}

//-----------------------------------------------------------------------------------------------------------------------
void cbTestMessage(dyn_string dps, dyn_anytype val)
{ 
  //DebugTN("cbMessage", dynlen(dps));
  
  dyn_mapping messages;
  for ( int i=1; i<=dynlen(dps); i++ ) 
  {
    mapping data;
    data["dp"]=dps[i];
    data["value"]=val[i];  
    dynAppend(messages, data);
  }
  
  dyn_string ids = mappingKeys(g_clients);
  for ( int i=1; i<=dynlen(ids); i++ ) 
  {
    pushMessages(ids[i], messages);  
  }    
}

//-----------------------------------------------------------------------------------------------------------------------
string xmlEncode(string n, string s)
{
  string xml;
  xmlrpcEncodeValue(s, xml, true);
  strreplace(xml, "<value>", "<"+n+">");
  strreplace(xml, "</value>", "</"+n+">");
  return xml;

}
//-----------------------------------------------------------------------------------------------------------------------
string createTextMessage(string sType, const string &text)
{
  mapping data;
  data["value"]=text;
  return createDataMessage(sType, makeDynMapping(data));
}

string createDataMessage(string sType, const dyn_mapping &data)
{ /*
  string xml;
  xml="<?xml version=\"1.0\" encoding=\"utf-8\"?>";
  xml+="<Message xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
  xml+=xmlEncode("Vers", "1.0");
  xml+=xmlEncode("Type", sType);  
  xml+=xmlEncode("Data", jsonEncode(data));
  xml+="</Message>";  
  return xml;
  */
  mapping message = makeMapping("Vers", "1.0", "Type", sType, "Data", jsonEncode(data));
  return jsonEncode(message);    
  //string json = jsonEncode(message);  
  //DebugTN("createDataMessage len="+strlen(json));
  //return json;
}

//-----------------------------------------------------------------------------------------------------------------------
string cbConnect(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN(user, ip, headerNames, headerValues);
  string cid = addClient();

  mapping data;
  data["cid"]=cid;

  return createDataMessage("C", makeDynMapping(data));
}

//-----------------------------------------------------------------------------------------------------------------------
string cbStream(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN(user, ip, dsNames, dsValues);
  
  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;  
  
  updateClient(cid);
  dyn_mapping messages;
  for ( int i=1; i<=10; i++ ) 
  {
    if ( popMessages(cid, messages) ) 
    {
      g_message_counter+=dynlen(messages);
      return createDataMessage("M", messages);
    }
    else 
    {
      delay(0,10);
    }
  }
  return createTextMessage("", "");
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_evalScript(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN(user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;    
  
  int i;
  if ( (i=dynContains(dsNames, "f"))==0 ) 
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no function given")));      
    
  string function=dsValues[i];
  mapping parameter = makeMapping();
       
  if ( (i=dynContains(dsNames, "p"))>0 ) 
    parameter=jsonDecode(dsValues[i]);
      
  anytype result;
  try 
  {
    evalScript(result, function, makeDynString(), parameter);
    if ( getType(result) == MAPPING_VAR ) 
      return createDataMessage("R", makeDynMapping(makeMapping("type", "mapping", "mapping", result)));              
    else
      return createDataMessage("R", makeDynMapping(makeMapping("type", "anytype", "anytype", result)));        
  } 
  catch 
  {
    DebugTN("cbExecute_evalScript", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  }   
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_dpNames(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN(user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;     
  
  int i;
  
  string pattern;
  if ( (i=dynContains(dsNames, "pattern"))>0 ) 
    pattern=dsValues[i];
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no pattern given")));        

  string type="";
  if ( (i=dynContains(dsNames, "type"))>0 ) 
    type=dsValues[i];
      
  try 
  {
    dyn_string names;    
    if ( type != "" ) names=dpNames(pattern, type);
    else names=dpNames(pattern);
    return createDataMessage("R", makeDynMapping(makeMapping("type", "tuple", "names", names)));        
  } 
  catch 
  {
    DebugTN("cbExecute_dpNames", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  } 
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_dpGetValues(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN(user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;   
  
  int i;

  dyn_string parameter;      
  if ( (i=dynContains(dsNames, "p"))>0 ) 
    parameter=jsonDecode(dsValues[i]);
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no parameter given")));          
      
  try 
  {
    dyn_anytype values;    
    dpGet(parameter, values);
    return createDataMessage("R", makeDynMapping(makeMapping("type", "tuple", "values", values)));        
  } 
  catch 
  {
    DebugTN("cbExecute_dpGetValues", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  }      
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_dpGetMapping(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN(user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;    
  
  int i;

  dyn_string parameter;      
  if ( (i=dynContains(dsNames, "p"))>0 ) 
    parameter=jsonDecode(dsValues[i]);
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no parameter given")));            
      
  try 
  {
    mapping result;    
    dyn_anytype resultArray;
    
    dpGet(parameter, resultArray);
    for ( i=1; i<=dynlen(resultArray); i++ ) 
    {
      result[parameter[i]]=resultArray[i];
    }
    return createDataMessage("R", makeDynMapping(makeMapping("type", "mapping", "mapping", result)));        
  } 
  catch 
  {
    DebugTN("cbExecute_dpGet", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  }
    
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_dpSet(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN(user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;  
  
  int i;

  mapping parameter;
  if ( (i=dynContains(dsNames, "p"))>0 ) 
    parameter=jsonDecode(dsValues[i]);
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no parameter given")));          
      
  try 
  {
    dyn_string dps=parameter["names"];
    //DebugTN(dps, parameter["val"]);
    int result = dpSet(dps, parameter["values"]);
    return createDataMessage("R", makeDynMapping(makeMapping("type", "result", "result", result, "message", "")));        
  } 
  catch 
  {
    DebugTN("cbExecute_dpGet", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  }
   
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_dpConnect(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN("cbExecute_dpConnect", user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;   
  
  int i, j;
      
  string fid;      
  if ( (i=dynContains(dsNames, "fid"))>0 ) 
    fid=dsValues[i];
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no callback given")));  

  dyn_string p = makeDynString();          
  if ( (i=dynContains(dsNames, "p"))>0 ) 
    p=jsonDecode(dsValues[i]);
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -2, "message", "no parameter given")));             

  bool answer = false;
  if ( (i=dynContains(dsNames, "a"))>0 ) 
    answer=dsValues[i];      
      
  dyn_anytype result;
  try 
  {
    DebugTN("cbExecute_dpConnect", cid, fid, p);    
    mapping d = makeMapping("cid", cid, "fid", fid);
    int result = dpConnectUserData("cbExecute_dpConnectCB", d, answer, p);
    if ( result == 0 ) g_connects[cid][fid] = makeMapping("t", "c", "d", d, "p", p);
    return createDataMessage("R", makeDynMapping(makeMapping("type", "result", "result", result, "message", "")));        
  } 
  catch 
  {
    DebugTN("cbExecute_dpConnect", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  }  
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_dpDisconnect(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN("cbExecute_dpDisconnect", user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;   
  
  int i;
    
  string fid;
  if ( (i=dynContains(dsNames, "fid"))>0 ) 
    fid=dsValues[i];
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no callback given")));  
  
  if ( !mappingHasKey(g_connects[cid], fid) )
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -2, "message", "function not connected")));
      
  try 
  {
    mapping d = g_connects[cid][fid]["d"];
    dyn_string p = g_connects[cid][fid]["p"];
    DebugTN("cbExecute_dpDisonnect", cid, fid, p);        
    int result = dpDisconnectUserData("cbExecute_dpConnectCB", d, p);
    mappingRemove(g_connects[cid], fid);
    return createDataMessage("R", makeDynMapping(makeMapping("type", "result", "result", result, "message", "")));        
  } 
  catch 
  {
    DebugTN("cbExecute_dpDisconnect", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  }
}

//-----------------------------------------------------------------------------------------------------------------------
void cbExecute_dpConnectCB(mapping data, dyn_string dps, dyn_anytype val)
{
  //DebugTN("cbExecute_dpConnectCB", data, dps, val); 
  pushMessage(data["cid"], makeMapping("fid", data["fid"], "type", "tuple", "names", dps, "values", val));
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_dpQueryConnect(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN("cbExecute_dpConnect", user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;   
  
  int i, j;
      
  string fid;      
  if ( (i=dynContains(dsNames, "fid"))>0 ) 
    fid=dsValues[i];
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no callback given")));  

  mapping parameter;          
  if ( (i=dynContains(dsNames, "p"))>0 ) 
    parameter=jsonDecode(dsValues[i]);
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -2, "message", "no parameter given")));             

  bool answer = false;
  if ( (i=dynContains(dsNames, "a"))>0 ) 
    answer=dsValues[i];      
      
  try 
  {
    DebugTN("cbExecute_dpQueryConnectSingle", cid, fid, parameter);    
    mapping data = makeMapping("cid", cid, "fid", fid);
    string query = parameter["query"];
    int result = dpQueryConnectSingle("cbExecute_dpQueryConnectCB", answer, data, query);
    if ( result == 0 ) g_connects[cid][fid] = makeMapping("t", "q", "d", data, "p", parameter);
    return createDataMessage("R", makeDynMapping(makeMapping("type", "result", "result", result, "message", "")));        
  } 
  catch 
  {
    DebugTN("cbExecute_dpQueryConnectSingle", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  }  
}

//-----------------------------------------------------------------------------------------------------------------------
string cbExecute_dpQueryDisconnect(dyn_string dsNames, dyn_string dsValues, string user, string ip, dyn_string headerNames, dyn_string headerValues)
{
  //DebugTN("cbExecute_dpDisconnect", user, ip, dsNames, dsValues);

  string cid, err;
  if ( (cid=getClient(dsNames, dsValues, err)) == "" )
    return err;   
  
  int i;
    
  string fid;
  if ( (i=dynContains(dsNames, "fid"))>0 ) 
    fid=dsValues[i];
  else
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -1, "message", "no callback given")));  
  
  if ( !mappingHasKey(g_connects[cid], fid) )
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -2, "message", "function not connected")));
      
  try 
  {
    mapping data = g_connects[cid][fid]["d"];
    mapping parameter = g_connects[cid][fid]["p"];
    DebugTN("cbExecute_dpQueryDisonnect", cid, fid, parameter);        
    int result = dpQueryDisconnect("cbExecute_dpQueryConnectCB", data);
    mappingRemove(g_connects[cid], fid);
    return createDataMessage("R", makeDynMapping(makeMapping("type", "result", "result", result, "message", "")));        
  } 
  catch 
  {
    DebugTN("cbExecute_dpQueryDisconnect", "EXCEPTION");
    string msg = getLastException();
    return createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -99, "message", msg)));
  }
}

//-----------------------------------------------------------------------------------------------------------------------
void cbExecute_dpQueryConnectCB(mapping data, dyn_dyn_anytype table)
{
  //DebugTN("cbExecute_dpQueryConnectCB", data, table); 
  pushMessage(data["cid"], makeMapping("fid", data["fid"], "type", "table", "table", table));
}

//-----------------------------------------------------------------------------------------------------------------------
bool pushMessage(string cid, const mapping &message) synchronized ( g_clients )
{
  if ( mappingHasKey(g_messages, cid) ) 
  {
    dynAppend(g_messages[cid], message);
    return true;
  }
  else
    return false;
}

//-----------------------------------------------------------------------------------------------------------------------
bool pushMessages(string cid, const dyn_mapping &messages) synchronized ( g_clients )
{
  if ( mappingHasKey(g_messages, cid) ) 
  {
    dynAppend(g_messages[cid], messages);
    return true;
  }
  else
    return false;
}

//-----------------------------------------------------------------------------------------------------------------------
bool popMessage(string cid, mapping &message) synchronized ( g_clients )
{
  if ( dynlen(g_messages[cid]) == 0 )
  {
    message=makeMapping();
    return false;
  }
  else
  {
    message = g_messages[cid][1];
    dynRemove(g_messages[cid],1);    
    return true;
  }    
}

//-----------------------------------------------------------------------------------------------------------------------
bool popMessages(string cid, dyn_mapping &messages) synchronized ( g_clients )
{
  bool ret;
  mapping message;
  while ( popMessage(cid, message) )
    dynAppend(messages, message);
  return dynlen(messages);
}

//-----------------------------------------------------------------------------------------------------------------------
string addClient(string cid="") synchronized ( g_clients ) 
{
  time t = getCurrentTime();
  if ( cid == "" ) 
    cid=(string)(++g_id);
  g_messages[cid]=makeDynMapping();  
  g_clients[cid]=makeMapping("ConnectTime", t, "StreamTime", t);
  g_connects[cid]=makeMapping();
  return cid;
}

//-----------------------------------------------------------------------------------------------------------------------
string getClient(const dyn_string &dsNames, const dyn_string &dsValues, string &err)
{
  int i;
  if ( (i=dynContains(dsNames, "cid"))>0 ) 
  {
    string cid=dsValues[i];
    if ( mappingHasKey(g_clients, cid) )
    {
      err="";
      return cid;
    }
    else 
    {
      err=createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -100, "message", "unknown connection")));                
      return "";
    }
  } 
  else 
  {
    err=createDataMessage("E", makeDynMapping(makeMapping("type", "result", "result", -101, "message", "no connection given")));                    
    return "";
  }    
}


//-----------------------------------------------------------------------------------------------------------------------
bool removeClient(string cid) synchronized ( g_clients ) 
{
  int i;
  
  if ( mappingHasKey(g_clients, cid) ) 
  {
    mappingRemove(g_clients, cid);
    mappingRemove(g_messages, cid);

    // remove connects
    dyn_string keys = mappingKeys(g_connects[cid]);
    for ( i=1; i<=dynlen(keys); i++ ) 
    {
        string t = g_connects[cid][keys[i]]["t"];
        mapping d = g_connects[cid][keys[i]]["d"];
        dyn_string p = g_connects[cid][keys[i]]["p"];
        switch ( t ) 
        {
          case "c": dpDisconnectUserData("cbExecute_dpConnectCB", d, p); break;
          case "q": dpQueryDisconnect("cbExecute_dpQueryConnectCB", d); break;
        } 
    }
    mappingRemove(g_connects, cid);
    
    return true;
  } 
  else {
    return false;
  }
}

//-----------------------------------------------------------------------------------------------------------------------
void updateClient(string cid) synchronized ( g_clients ) 
{
  if ( mappingHasKey(g_clients, cid) )
  {
    g_clients[cid]["StreamTime"]=getCurrentTime();
  }
}

//-----------------------------------------------------------------------------------------------------------------------
void checkClients()
{
  int s_old=0;
  time t1 = getCurrentTime();
  while ( true ) 
  {
    synchronized ( g_clients ) 
    {
      int s=0;      
      int c=0;
      dyn_string ids = mappingKeys(g_clients);
      for ( int i=1; i<=dynlen(ids); i++ )
      {
        string cid = ids[i];
        
        s+=dynlen(g_messages[cid]);     
        c+=mappinglen(g_connects[cid]);
        
        if ( getCurrentTime() - g_clients[cid]["StreamTime"] > 5.0 ) {
          DebugTN("checkClients["+cid+"] timedout!");
          removeClient(cid);
        }
      }
      
      time t2=getCurrentTime();
      float sec=t2-t1;
      if ( s > 0 || s_old > 0 || sec >= 10.0 ) 
      {
        float sec=t2-t1;  
        DebugTN("checkClients;"+mappinglen(g_clients)+" clients;"+s+" messages in queues;"+c+" connects;"+(sec>0 ? (int)(g_message_counter/sec) : -1)+" messages/sec sent");
        g_message_counter=0;   
        t1=t2;  
        s_old=s;        
      }      
    }
    delay(1);
  }
}
