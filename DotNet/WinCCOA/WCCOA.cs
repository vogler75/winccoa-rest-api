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

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace WinCCOA
{
    public delegate void CallbackFunction(object sender, MessageResult e);

    public delegate void ListenerState(WCCOABase sender, bool listening);

    public class WCCOABase
    {
        protected String host = "";
        protected String url = "";
        protected String cid = "";
        protected uint fid = 0;

        public String getUrl { get { return this.url; } }
        public String getCid { get { return this.cid; } }
        public String getFid { get { return (++this.fid).ToString(); } }

        private Thread listener;
        private bool isListening = false;
        private bool stopListening = false;
        public event ListenerState ListenerStateChanged;

        volatile private Dictionary<String, CallbackObject> CallbackObjects = new Dictionary<String, CallbackObject>();

        //------------------------------------------------------------------------------------------------
        public WCCOABase(string host)
        {
            this.host = host;
            this.url = "http://" + host;
        }

        //------------------------------------------------------------------------------------------------
        public bool Open()
        {
            MessageBulk message = Request(this.url + "/Connect");
            if (message == null || message.Type != "C") return false;

            if (message.Items().Count == 0) return false;

            this.cid = message.Items()[0].getString("cid");

            Console.WriteLine("Connected to " + this.url + " with id " + this.cid);

            // TODO reconnect all CallbackObjects

            return true;
        }

        //------------------------------------------------------------------------------------------------
        public void ReconnectObjects()
        {
            foreach ( var obj in CallbackObjects )
            {
                obj.Value.Connect();
            }
        }

        //------------------------------------------------------------------------------------------------
        public void StartListener()
        {
            if (!isListening)
            {
                (listener = new Thread(this.Listener)).Start();
            }
        }

        //------------------------------------------------------------------------------------------------
        public void StopListener()
        {
            stopListening = true;
        }

        //------------------------------------------------------------------------------------------------
        private void setListenerState(bool listening)
        {
            if (isListening != listening)
            {
                isListening = listening;

                if (stopListening)
                    stopListening = false;

                if (ListenerStateChanged != null)
                    ListenerStateChanged(this, listening);
            }
        }

        //------------------------------------------------------------------------------------------------
        private void Listener()
        {
            Console.WriteLine("Listen to " + this.host + " with id " + this.cid);

            MessageBulk message;

            int cnt = 0;
            double d = 0;
            DateTime t1 = DateTime.Now;

            try
            {
                setListenerState(true);
                while (!stopListening)
                {
                    message = Request(this.url + "/Stream?cid=" + HttpUtility.UrlEncode(this.getCid));
                    if (message == null)
                    {
                        Console.WriteLine("Listener null message");
                    }
                    else if (message.Type != "")
                    {
                        //Console.WriteLine("Listener message: " + message.Data);

                        // error
                        if (message.Type == "E")
                        {
                            Console.WriteLine("Listener error: " + message);
                            stopListening = true;
                        }

                        // message / function callback
                        else if (message.Type == "M")
                        {      
                            foreach (var i in message.Items())
                                if ( CallbackObjects.ContainsKey(i.fid) ) 
                                    CallbackObjects[i.fid].Enqueue(this, i);

                            // metrics
                            cnt += message.Items().Count;
                            d = (DateTime.Now - t1).TotalSeconds;
                            if (d > 10)
                            {
                                Console.WriteLine("Listener " +  (cnt / d) + " messages/sec");
                                cnt = 0;
                                t1 = DateTime.Now;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Listener unknown type: " + message.Type);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Listener exception: " + e);
            }
            finally
            {
                setListenerState(false);
            }
            Console.WriteLine("Listener ended. ");
        }

        //------------------------------------------------------------------------------------------------
        internal String AddCallback(CallbackObject cb)
        {
            String fid = cb.getFid();
            if (cb.getFid() == null)
                fid = this.getFid;

            if (!CallbackObjects.ContainsKey(fid))
                CallbackObjects.Add(fid, cb);

            return fid;
        }

        internal bool RemoveCallback(String fid)
        {
            if (this.CallbackObjects.ContainsKey(fid))
            {
                this.CallbackObjects.Remove(fid);
                return true;
            }
            else
            {
                return false;
            }
        }

        //------------------------------------------------------------------------------------------------
        public MessageBulk Request(String url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    Stream stream = response.GetResponseStream();
                    MessageBulk message = new MessageBulk();                 

                    StreamReader streamreader = new StreamReader(stream);
                    string data = streamreader.ReadToEnd();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = MessageBulk.MAX_JSON_LENGTH; 
                    message = serializer.Deserialize<MessageBulk>(data);

                    stream.Close();
                    return message;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }

    //================================================================================================
    public class WCCOABasic : WCCOABase
    {

        public WCCOABasic(string host) : base(host)
        {

        }

        //------------------------------------------------------------------------------------------------
        public MessageResult evalScript(String fun, Dictionary<String, dynamic> par = null)
        {
            String serpar = "";
            if (par != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serpar = serializer.Serialize(par);
            }

            var message = Request(this.url + "/Execute/evalScript?cid=" + HttpUtility.UrlEncode(this.cid) + "&f=" + HttpUtility.UrlEncode(fun) + "&p=" + HttpUtility.UrlEncode(serpar));
            return message.Count > 0 ? new MessageResult(message[0]) : null;
        }

        //------------------------------------------------------------------------------------------------
        public MessageResult dpGet(List<String> dps)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String s = serializer.Serialize(dps);

            var message = Request(this.url + "/Execute/dpGetValues?cid=" + HttpUtility.UrlEncode(this.cid) + "&p=" + HttpUtility.UrlEncode(s));
            if ( message.Count > 0 )
            {
                var result = new MessageResult(message[0]);
                result.Names=dps; // no elements are returned by dpGet
                return result;
            }
            else 
            {
                return null;
            }
        }

        //------------------------------------------------------------------------------------------------
        public MessageResult dpNames(String dpPattern, String dpType="")
        {
            var message = Request(this.url + "/Execute/dpNames?cid=" + HttpUtility.UrlEncode(this.cid) + "&pattern=" + HttpUtility.UrlEncode(dpPattern) + "&type=" + HttpUtility.UrlEncode(dpType));
            return message.Count > 0 ? new MessageResult(message[0]) : null;
        }

        //------------------------------------------------------------------------------------------------
        public MessageResult dpGetAsMapping(List<String> dps)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String s = serializer.Serialize(dps);

            var message = Request(this.url + "/Execute/dpGetMapping?cid=" + HttpUtility.UrlEncode(this.cid) + "&p=" + HttpUtility.UrlEncode(s));
            return message.Count > 0 ? new MessageResult(message[0]) : null;
        }

        //------------------------------------------------------------------------------------------------
        public MessageResult dpSet(List<String> dps, List<dynamic> val)
        {
            var p = new Dictionary<String, dynamic>();

            p.Add("names", dps);
            p.Add("values", val);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String s = serializer.Serialize(p);

            var message = Request(this.url + "/Execute/dpSet?cid=" + HttpUtility.UrlEncode(this.cid) + "&p=" + HttpUtility.UrlEncode(s));
            return message.Count > 0 ? new MessageResult(message[0]) : null;
        }

        //------------------------------------------------------------------------------------------------
        public dpConnectValue dpConnectValue(CallbackFunction cb, List<String> dps, bool answer=false)
        {
            return new dpConnectValue(this, dps, cb, answer);
        }

        //------------------------------------------------------------------------------------------------
        public dpConnectQuery dpConnectQuery(CallbackFunction cb, String query, bool answer = false)
        {
            return new dpConnectQuery(this, query, cb, answer);
        }
    }

    //================================================================================================
    [Serializable]
    public class MessageBulk
    {
        public const int MAX_JSON_LENGTH = 16777216 /*16MB (default is 4MB)*/; 

        public string Vers { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }

        public override String ToString()
        {
            return Vers + "/" + Type + "/" + Data;
        }

        private List<MessageItem> ItemList;
        public int Count { get { return this.Items().Count; } }

        public List<MessageItem> Items()
        {
            return (this.ItemList == null ? this.Deserialize() : this.ItemList);
        }

        public MessageItem this[int idx]
        {
            get
            {
                return (Items().Count > 0 ? Items()[idx] : null);
            }
        }

        private List<MessageItem> Deserialize()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = MAX_JSON_LENGTH; 
            List<Dictionary<String, dynamic>> l;
            l = serializer.Deserialize<List<Dictionary<String, dynamic>>>(this.Data);
            this.ItemList = new List<MessageItem>();
            for (int i = 0; i < l.Count; i++)
                ItemList.Add(new MessageItem(l[i]));
            return this.ItemList;
        }
    }

    //================================================================================================
    public class MessageItem : EventArgs
    {
        protected Dictionary<String, dynamic> data;

        public MessageItem(Dictionary<String, dynamic> data)
        {
            this.data = data;
        }

        public MessageItem(MessageItem message)
        {
            this.data = message.data;
        }

        public override String ToString()
        {
            return data != null ? data.ToString() : "";
        }

        public String fid { get { return HasKey("fid") ? data["fid"] : ""; } }

        public bool HasKey(String key)
        {
            return (data != null && data.ContainsKey(key));
        }

        public int getInt(String key)
        {
            return HasKey(key) ? data[key] : null;
        }

        public String getString(String key) 
        { 
            return HasKey(key) ? data[key] : null; 
        }

        public ArrayList getArray(String key) 
        {
            return HasKey(key) ? data[key] : null; 
        }

        public dynamic getDynamic(String key)
        {
            return HasKey(key) ? data[key] : null;
        }
    }

    //================================================================================================
    public class MessageResult : MessageItem
    {
        public MessageResult(Dictionary<String, dynamic> data) : base(data)
        {
            Init();
        }

        public MessageResult(MessageItem message) : base (message)
        {
            Init();
        }

        private List<String> _names;

        enum ResultType : int { Unknown, Tuple, Table, Mapping, Result, Anytype };
        ResultType _type;

        private bool _hasNames = false;
        private bool _hasValues = false;        

        private void Init()
        {
            _type = ResultType.Unknown;
            _names = new List<String>();
            if ( HasKey("type") )
            {
                String type = this.getString("type");
                switch ( type )
                {
                    case "tuple": 
                        if (HasKey("names") || HasKey("values"))
                        {
                            _type = ResultType.Tuple;
                            if (HasKey("names"))
                            {
                                _hasNames = true;
                                for (int i = 0; i < getArray("names").Count; i++)
                                    _names.Add(getArray("names")[i] as String);
                            }
                            if (HasKey("values"))
                            {
                                _hasValues = true;
                            }
                        }
                        break;
                    case "table":
                        if (HasKey("table"))
                        {
                            _type = ResultType.Table;
                        }
                        break;
                    case "mapping":
                        if (HasKey("mapping"))
                        {
                            _type = ResultType.Mapping;
                            _hasNames = true;
                            foreach (var key in (getDynamic("mapping") as Dictionary<String, dynamic>).Keys)
                                _names.Add(key);
                        }
                        break;
                    case "result":
                        if (HasKey("result"))
                        {
                            _type = ResultType.Result;
                        }
                        break;
                    case "anytype":
                        if (HasKey("anytype"))
                        {
                            _type = ResultType.Anytype;
                        }
                        break;
                }
            }
        }

        public List<String> Names
        {
            get
            {
                return _names;
            }
            set
            {
                _names = value;
            }
        }

        public int Count
        {
            get
            {
                switch ( _type )
                {
                    case ResultType.Tuple     : return _hasNames ? getArray("names").Count : _hasValues ? getArray("values").Count : 0;
                    case ResultType.Table     : return ((ArrayList)getDynamic("table")).Count;
                    case ResultType.Mapping   : return _names.Count;
                    case ResultType.Result    : return 1;
                    case ResultType.Anytype   : return 1;
                    default: return 0;
                }
            }
        }

        public ArrayList Values
        {
            get 
            {
                return _hasValues ? getArray("values") : new ArrayList();
            }            
        }

        public ArrayList this[int idx]
        {
            get
            {
                return _type == ResultType.Table ? (ArrayList)(((ArrayList)getDynamic("table"))[idx])  : null;
            }
        }

        public int IndexOf(String name)
        {
            return _hasNames ? Names.IndexOf(name) : 0;
        }

        public dynamic ValueOf(String name)
        {
            return _hasValues ? Values[IndexOf(name)] : null;
        }

        public Dictionary<String, dynamic> Mapping
        {
            get
            {
                return _type == ResultType.Mapping ? (Dictionary<String, dynamic>)getDynamic("mapping") : null;
            }
        }

        public int ResultCode
        {
            get
            {
                return _type == ResultType.Result ? getInt("result") : 0;
            }
        }

        public String ResultMessage
        {
            get
            {
                return _type == ResultType.Result ? getString("message") : "";
            }
        }

        public dynamic Anytype
        {
            get
            {
                return _type == ResultType.Anytype ? getDynamic("anytype") : null;
            }
        }
    }

    //================================================================================================
    public abstract class CallbackObject
    {
        protected WCCOABasic oa;
        protected String fid;
        protected bool answer;

        public event CallbackFunction Callback;

        private Thread MessageWorker;
        private Queue<MessageItem> MessageQueue = new Queue<MessageItem>();

        private bool connected = false;

        public CallbackObject(WCCOABasic oa, bool answer = false)
        {
            this.oa = oa;
            this.answer = answer;
        }

        public CallbackObject(WCCOABasic oa, CallbackFunction cb, bool answer = false)
            : this(oa, answer)
        {
            this.Callback += cb;
        }

        ~CallbackObject()
        {
            this.Disconnect();
        }

        public String getFid()
        {
            return this.fid;
        }

        private void setFid(String fid)
        {
            this.fid = fid;
        }

        abstract protected bool ConnectImplementation();

        public void Connect()
        {
            this.fid = oa.AddCallback(this);
            this.MessageQueue.Clear();
            connected = ConnectImplementation();
            if ( connected )
                (MessageWorker = new Thread(this.Dequeue)).Start();
        }

        abstract protected void DisconnectImplementation();

        public void Disconnect()
        {
            if (connected)
            {
                DisconnectImplementation();
                oa.RemoveCallback(this.fid);
                connected = false;
            }
        }

        public void Enqueue(object sender, MessageItem m)
        {
            lock (MessageQueue)
            {
                MessageQueue.Enqueue(m);
                Monitor.Pulse(MessageQueue);
            }
        }

        private void Dequeue()
        {
            MessageItem m;
            while (true)
            {
                lock (MessageQueue)
                {
                    Monitor.Wait(MessageQueue);
                }
                do
                {
                    lock (MessageQueue)
                    {
                        m = (MessageQueue.Count > 0) ? MessageQueue.Dequeue() : null;
                    }
                    if (m != null && this.Callback != null)
                    {
                        this.Callback(this, new MessageResult(m));
                    }

                } while (m != null);
            }
        }

        public int getQueueLength()
        {
            lock (MessageQueue) {
                return MessageQueue.Count;
            }
        }
    }

    //================================================================================================
    public class dpConnectValue : CallbackObject
    {
        private List<String> dps;

        public dpConnectValue(WCCOABasic oa, List<String> dps, bool answer=false) 
            : base(oa, answer)
        {
            this.dps = dps;
            this.Connect();
        }

        public dpConnectValue(WCCOABasic oa, List<String> dps, CallbackFunction callback, bool answer = false) 
            : base(oa, callback, answer)
        {
            this.dps = dps;
            this.Connect();
        }

        protected override bool ConnectImplementation()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String s = serializer.Serialize(dps);
            var msg = oa.Request(oa.getUrl + "/Execute/dpConnect?cid=" + HttpUtility.UrlEncode(oa.getCid) + "&fid=" + HttpUtility.UrlEncode(this.fid) + "&a=" + (answer ? 1 : 0) + "&p=" + HttpUtility.UrlEncode(s));
            return msg.Count > 0 && new MessageResult(msg[0]).ResultCode == 0 ? true : false;
        }

        protected override void DisconnectImplementation()
        {
            oa.Request(oa.getUrl + "/Execute/dpDisconnect?cid=" + HttpUtility.UrlEncode(oa.getCid) + "&fid=" + HttpUtility.UrlEncode(this.fid));
        }
    }

    //================================================================================================
    public class dpConnectQuery : CallbackObject
    {
        private string query;

        public dpConnectQuery(WCCOABasic oa, String query, bool answer=false) 
            : base(oa, answer)
        {
            this.query = query;
            this.Connect();
        }

        public dpConnectQuery(WCCOABasic oa, String query, CallbackFunction callback, bool answer = false) 
            : base(oa, callback, answer)
        {
            this.query = query;
            this.Connect();
        }

        protected override bool ConnectImplementation()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String s = serializer.Serialize(new Dictionary<String, String> { {"query", query} });
            Console.WriteLine("dpQueryConnectObject.Connect: " + s);
            var msg = oa.Request(oa.getUrl + "/Execute/dpQueryConnect?cid=" + HttpUtility.UrlEncode(oa.getCid) + "&fid=" + HttpUtility.UrlEncode(this.fid) + "&a=" + (answer ? 1 : 0) + "&p=" + HttpUtility.UrlEncode(s));
            return msg.Count > 0 && new MessageResult(msg[0]).ResultCode == 0 ? true : false;
        }

        protected override void DisconnectImplementation()
        {
            oa.Request(oa.getUrl + "/Execute/dpQueryDisconnect?cid=" + HttpUtility.UrlEncode(oa.getCid) + "&fid=" + HttpUtility.UrlEncode(this.fid));
        }
    }
}
