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
using WinCCOA;

namespace Client1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage <host:port>");
                return;
            }
            WCCOABasic oa = new WCCOABasic(args[0]);

            if ( oa.Open() )
            {
                oa.StartListener();
                oa.ListenerStateChanged += (WCCOABase sender, bool listening) =>
                {
                    Console.WriteLine("Listener changed state " + listening);
                    if (!listening)
                    {
                        oa.Open();
                        oa.StartListener();
                        oa.ReconnectObjects();
                    }
                };

                MessageResult res;

                // --------------------------------------------------------------------------------------------
                // dpQueryConnect
                int cnt2 = 0;
                DateTime t2 = DateTime.Now;
                Console.WriteLine("=== dpQueryConnect ===");
                var c2 = oa.dpConnectQuery(
                    (object sender, MessageResult m) =>
                    {
                        for (int i = 1; i < m.Count; i++)
                        {
                            cnt2++;

                            //if (++cnt % 100 == 0)                           
                            //    Console.WriteLine(cnt + " calls");
                            //Thread.Sleep((int)(100*(new Random()).NextDouble()));
                            
                            Console.WriteLine("Callback[" + i + "]" + m[i][0] + " " + m[i][1] + " " + m[i][2]);
                        }

                        double d;
                        if ( (d=DateTime.Now.Subtract(t2).Seconds) > 3 )
                        {
                            Console.WriteLine(cnt2 / d + " values/sec");
                            t2 = DateTime.Now;
                            cnt2 = 0;
                        }
                    },
                    "SELECT '_original.._value', '_original.._stime' FROM '*.**'",
                    false
                );
                Console.WriteLine("press key to disconnect"); Console.ReadKey();
                c2.Disconnect();


                // --------------------------------------------------------------------------------------------
                // dpNames
                Console.WriteLine("=== dpNames ===");
                res = oa.dpNames("*", "ExampleDP_Float");
                if (res.Count > 0)
                {
                    for (int i = 0; i < res.Count && i < 100; i++)
                    {
                        Console.WriteLine(res.Names[i]);
                        //oa.dpConnectValue((object sender, MessageResult m) => { Console.WriteLine("\r"+m.Names[0] +" => " + m.Values[0]); }, new List<String> { res.Names[i] });
                    }
                }
                Console.WriteLine("press key to continue");
                Console.ReadKey();

                // --------------------------------------------------------------------------------------------
                // dpGet
                Console.WriteLine("=== dpGet ===");
                res = oa.dpGet(new List<String> { "ExampleDP_Rpt1.", "ExampleDP_Rpt2.", "ExampleDP_Rpt3." });
                if (res.Count > 0)
                {
                    Console.WriteLine("ValueByIdx: " + res.Values[0]);
                    Console.WriteLine("ValueOf: " + res.ValueOf("Test_1.Time"));
                }
                Console.WriteLine("press key to continue");
                Console.ReadKey();

                // --------------------------------------------------------------------------------------------
                // dpGetMapping
                Console.WriteLine("=== dpGetAsMapping ===");
                res = oa.dpGetAsMapping(new List<String> { "ExampleDP_Rpt1.", "ExampleDP_Rpt2.", "ExampleDP_Rpt3." });
                if (res.Mapping != null)
                {
                    Console.WriteLine("Mapping: " + res.Mapping["ExampleDP_Rpt1."]);

                    // Dyn Example
                    //for (int i = 0; i < ((ArrayList)(res.Mapping["Test_1.DynFloat"])).Count; i++)
                    //    Console.WriteLine("DynFloat[" + i + "]" + res.Mapping["Test_1.DynFloat"][i]);
                }
                Console.WriteLine("press key to continue");
                Console.ReadKey();

                // --------------------------------------------------------------------------------------------
                // evalScript
                Console.WriteLine("=== evalScript ===");
                Dictionary<String, dynamic> p = new Dictionary<String, dynamic>() { { "dp", "ExampleDP_Rpt2." } };

                res = oa.evalScript("anytype main(mapping par) { anytype res; dpGet(par[\"dp\"], res); return res; }", p);
                Console.WriteLine("Anytype: " + res.Anytype);

                res = oa.evalScript("mapping main(mapping par) { anytype res; dpGet(par[\"dp\"], res); return makeMapping(\"res\", res); }", p);
                Console.WriteLine("Mapping: " + res.Mapping["res"]);

                Console.WriteLine("press key to continue");
                Console.ReadKey();

                // --------------------------------------------------------------------------------------------
                // dpConnect
                Console.WriteLine("=== dpConnectValue ===");
                var c1 = oa.dpConnectValue(
                    (object sender, MessageResult m) =>
                    {
                        for (int i = 0; i < m.Count; i++)
                        {
                            Console.WriteLine("Callback1[" + i + "]" + m.Names[i] + " => " + m.Values[i]);
                        }
                    },
                    new List<String> { "ExampleDP_Rpt1.", "ExampleDP_Rpt2.", "ExampleDP_Rpt3." }
                );
                Console.WriteLine("press key to disconnect"); Console.ReadKey();
                c1.Disconnect();

                // --------------------------------------------------------------------------------------------
                Console.WriteLine("press key to stop listener"); Console.ReadKey();
                oa.StopListener();
            }
        }

    }


}
