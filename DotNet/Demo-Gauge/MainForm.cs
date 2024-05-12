using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinCCOA;
using System.Threading;

namespace AGaugeDemo
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.AGaugeLabel label1;
        private System.Windows.Forms.AGaugeLabel label2;
        private System.Windows.Forms.AGaugeLabel label3;

        private System.Windows.Forms.AGaugeRange alert;

        bool trackBar_dpSet = true;

        WCCOABasic oa = new WCCOABasic("localhost:8080"); 

        public MainForm()
        {
            InitializeComponent();

            label1 = aGauge1.GaugeLabels.FindByName("GaugeLabel1");
            label2 = aGauge2.GaugeLabels.FindByName("GaugeLabel2");
            label3 = aGauge3.GaugeLabels.FindByName("GaugeLabel3");

            alert = aGauge1.GaugeRanges.FindByName("AlertRange");

            if (oa.Open())
            {
                // Gauge 1
                int cnt2 = 0;
                DateTime t2 = DateTime.Now;
                var c2 = oa.dpConnectQuery(
                    (object sender, MessageResult m) =>
                    {
                        cnt2++;
                        double d = (DateTime.Now - t2).TotalSeconds;
                        if (d >= 1)
                        {
                            this.Invoke((MethodInvoker)delegate // runs on UI thread
                            {
                                float f = (float)(cnt2 / d);                                
                                if (f > aGauge1.MaxValue)
                                {
                                    aGauge1.MaxValue = f;
                                    aGauge1.GaugeRanges.FindByName("AlertRange").EndValue = f;
                                }
                                aGauge1.Value = f;
                                label1.Text = (int)f + "v/s\n("+((dpConnectQuery)sender).getQueueLength().ToString() + ")";

                            });
                            cnt2 = 0;
                            t2 = DateTime.Now;
                        }
                    },

                    //"SELECT '_original.._value', '_original.._stime' FROM 'PerfTest_*'",
                    "SELECT '_original.._value', '_original.._stime' FROM '*.**'",
                    false
                );


                // dpConnect Version 2 with C# events
                //var c1 = new dpConnectValue(oa, new List<String> { "PerfTest_1.Float" });
                //c1.Callback += (object sender, MessageResult m) =>
                //{
                //    for (int i = 0; i < m.Count; i++)
                //    {
                //        //Console.WriteLine("Callback2[" + i + "]" + m.getArrayValue("dps")[i] + " => " + m.getArrayValue("val")[i]);
                //        this.Invoke((MethodInvoker)delegate
                //        {
                //            aGauge1.Value = (float)Convert.ToDouble(m.Values[i]); // runs on UI thread
                //        });                   
                //    }
                //};

                // dpConnect Version 2 with C# events
                DateTime t3 = DateTime.Now;
                var c3 = new dpConnectValue(oa, new List<String> { "ExampleDP_Rpt2." });
                c3.Callback += (object sender, MessageResult m) =>
                {
                    double d = (DateTime.Now - t3).TotalSeconds;
                    if (d >= 0.1)
                    {
                        t3 = DateTime.Now;
                        for (int i = 0; i < m.Count; i++)
                        {
                            //Console.WriteLine("Callback2[" + i + "]" + m.getArrayValue("dps")[i] + " => " + m.getArrayValue("val")[i]);
                            this.Invoke((MethodInvoker)delegate
                            {
                                aGauge2.Value = (float)Convert.ToDouble(m.Values[i]); // runs on UI thread
                            });
                        }
                    }
                };

                // Gauge 3
                var dps = new List<String> { "ExampleDP_Rpt1." };
                var c0 = new dpConnectValue(oa, dps, true);
                c0.Callback += (object sender, MessageResult m) =>
                {
                    for (int i = 0; i < m.Count; i++)
                    {
                        //Console.WriteLine("Callback2[" + i + "]" + m.getArrayValue("dps")[i] + " => " + m.getArrayValue("val")[i]);
                        this.Invoke((MethodInvoker)delegate
                        {
                            int v = (int)Convert.ToInt32(m.Values[i]);
                            if (trackBar1.Value != v)
                            {
                                trackBar_dpSet = false;
                                trackBar1.Value = v; // runs on UI thread
                            }
                            aGauge3.Value = v; // runs on UI thread
                        });
                    }
                };

                oa.StartListener();
                oa.ListenerStateChanged += (WCCOABase sender, bool listening) =>
                {
                    Console.WriteLine("Listener changed state " + listening);
                    if (!listening) {
                        oa.Open();
                        oa.StartListener();
                        oa.ReconnectObjects();
                    }
                };
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar_dpSet)
                oa.dpSet(new List<String> { "ExampleDP_Rpt1." }, new List<dynamic> { trackBar1.Value });
            else
                trackBar_dpSet = true;
        }

        private void aGauge1_ValueChanged(object sender, EventArgs e)
        {
            //label1.Text = aGauge1.Value.ToString();
        }

        private void aGauge2_ValueChanged(object sender, EventArgs e)
        {
            label2.Text = aGauge2.Value.ToString();
        }

        private void aGauge3_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = aGauge3.Value.ToString();
        }

        private void aGauge1_ValueInRangeChanged(object sender, System.Windows.Forms.ValueInRangeChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("InRange Event.");
            if (e.Range == alert)
            {
                panel1.BackColor = e.InRange ? Color.Red : Color.FromKnownColor(KnownColor.Control);
            }
        }
    }
}
