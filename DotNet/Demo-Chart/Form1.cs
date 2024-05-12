using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WinCCOA;

namespace AChart1
{
    public partial class Form1 : Form
    {
        //private System.ComponentModel.IContainer components = null;
        System.Windows.Forms.DataVisualization.Charting.Chart chart1;

        public Form1()
        {
            InitializeComponent();
            InitializeComponentChart();

            WCCOABasic oa = new WCCOABasic("localhost:8080");
            if (oa.Open())
            {
                // dpConnect with C# events
                var c1 = new dpConnectValue(oa, new List<String> { "ExampleDP_Rpt1.", "ExampleDP_Rpt2.", "ExampleDP_Rpt3.", "ExampleDP_Rpt4." });
                c1.Callback += (object sender, MessageResult m) =>
                {
                    for (int i = 0; i < m.Count; i++)
                    {
                        //Console.WriteLine("Callback2[" + i + "]" + m.getArrayValue("dps")[i] + " => " + m.getArrayValue("val")[i]);
                        this.Invoke((MethodInvoker)delegate // runs on UI thread
                        {
                            string s = (string)m.Names[i];
                            double f = (float)Convert.ToDouble(m.Values[i]);
                            if (chart1.Series[s].Points.Count >= chart1.ChartAreas[0].AxisX.Maximum)
                                chart1.Series[s].Points.RemoveAt(0);
                            chart1.Series[s].Points.AddY(f);
                        });
                    }
                };

                oa.StartListener();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "System1:ExampleDP_Rpt1.:_online.._value",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line,
            };
            var series2 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "System1:ExampleDP_Rpt2.:_online.._value",
                Color = System.Drawing.Color.Red,
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line,
            };
            var series3 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "System1:ExampleDP_Rpt3.:_online.._value",
                Color = System.Drawing.Color.Black,
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line,
            };
            var series4 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "System1:ExampleDP_Rpt4.:_online.._value",
                Color = System.Drawing.Color.Tomato,
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line,
            };

            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);

            chart1.ChartAreas[0].AxisX.Maximum = 500;

            chart1.Invalidate();
        }

        private void InitializeComponentChart()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            //
            // chart1
            //
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 50);
            this.chart1.Name = "chart1";
            // this.chart1.Size = new System.Drawing.Size(284, 212);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Chart";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
        }
    }   
}
