namespace AGaugeDemo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.AGaugeLabel aGaugeLabel1 = new System.Windows.Forms.AGaugeLabel();
            System.Windows.Forms.AGaugeRange aGaugeRange1 = new System.Windows.Forms.AGaugeRange();
            System.Windows.Forms.AGaugeLabel aGaugeLabel2 = new System.Windows.Forms.AGaugeLabel();
            System.Windows.Forms.AGaugeRange aGaugeRange2 = new System.Windows.Forms.AGaugeRange();
            System.Windows.Forms.AGaugeRange aGaugeRange3 = new System.Windows.Forms.AGaugeRange();
            System.Windows.Forms.AGaugeRange aGaugeRange4 = new System.Windows.Forms.AGaugeRange();
            System.Windows.Forms.AGaugeLabel aGaugeLabel3 = new System.Windows.Forms.AGaugeLabel();
            System.Windows.Forms.AGaugeRange aGaugeRange5 = new System.Windows.Forms.AGaugeRange();
            System.Windows.Forms.AGaugeRange aGaugeRange6 = new System.Windows.Forms.AGaugeRange();
            System.Windows.Forms.AGaugeRange aGaugeRange7 = new System.Windows.Forms.AGaugeRange();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.aGauge1 = new System.Windows.Forms.AGauge();
            this.panel1 = new System.Windows.Forms.Panel();
            this.aGauge2 = new System.Windows.Forms.AGauge();
            this.aGauge3 = new System.Windows.Forms.AGauge();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 20;
            this.trackBar1.Location = new System.Drawing.Point(15, 12);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 238);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // aGauge1
            // 
            this.aGauge1.BackColor = System.Drawing.SystemColors.Control;
            this.aGauge1.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge1.BaseArcRadius = 80;
            this.aGauge1.BaseArcStart = 135;
            this.aGauge1.BaseArcSweep = 270;
            this.aGauge1.BaseArcWidth = 2;
            this.aGauge1.Center = new System.Drawing.Point(100, 100);
            aGaugeLabel1.Color = System.Drawing.SystemColors.WindowText;
            aGaugeLabel1.Font = new System.Drawing.Font("Verdana", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            aGaugeLabel1.Name = "GaugeLabel1";
            aGaugeLabel1.Position = new System.Drawing.Point(85, 130);
            aGaugeLabel1.Text = "0";
            this.aGauge1.GaugeLabels.Add(aGaugeLabel1);
            aGaugeRange1.Color = System.Drawing.Color.Red;
            aGaugeRange1.EndValue = 200F;
            aGaugeRange1.InnerRadius = 70;
            aGaugeRange1.InRange = false;
            aGaugeRange1.Name = "AlertRange";
            aGaugeRange1.OuterRadius = 80;
            aGaugeRange1.StartValue = 160F;
            this.aGauge1.GaugeRanges.Add(aGaugeRange1);
            this.aGauge1.Location = new System.Drawing.Point(265, 12);
            this.aGauge1.MaxValue = 200F;
            this.aGauge1.MinValue = 0F;
            this.aGauge1.Name = "aGauge1";
            this.aGauge1.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Yellow;
            this.aGauge1.NeedleColor2 = System.Drawing.Color.Olive;
            this.aGauge1.NeedleRadius = 80;
            this.aGauge1.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.aGauge1.NeedleWidth = 2;
            this.aGauge1.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge1.ScaleLinesInterInnerRadius = 73;
            this.aGauge1.ScaleLinesInterOuterRadius = 80;
            this.aGauge1.ScaleLinesInterWidth = 1;
            this.aGauge1.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge1.ScaleLinesMajorInnerRadius = 70;
            this.aGauge1.ScaleLinesMajorOuterRadius = 80;
            this.aGauge1.ScaleLinesMajorStepValue = 20F;
            this.aGauge1.ScaleLinesMajorWidth = 2;
            this.aGauge1.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.aGauge1.ScaleLinesMinorInnerRadius = 75;
            this.aGauge1.ScaleLinesMinorOuterRadius = 80;
            this.aGauge1.ScaleLinesMinorTicks = 1;
            this.aGauge1.ScaleLinesMinorWidth = 1;
            this.aGauge1.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge1.ScaleNumbersFormat = null;
            this.aGauge1.ScaleNumbersRadius = 95;
            this.aGauge1.ScaleNumbersRotation = 0;
            this.aGauge1.ScaleNumbersStartScaleLine = 0;
            this.aGauge1.ScaleNumbersStepScaleLines = 1;
            this.aGauge1.Size = new System.Drawing.Size(205, 180);
            this.aGauge1.TabIndex = 0;
            this.aGauge1.Text = "aGauge1";
            this.aGauge1.Value = 0F;
            this.aGauge1.ValueChanged += new System.EventHandler(this.aGauge1_ValueChanged);
            this.aGauge1.ValueInRangeChanged += new System.EventHandler<System.Windows.Forms.ValueInRangeChangedEventArgs>(this.aGauge1_ValueInRangeChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(287, 198);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 28);
            this.panel1.TabIndex = 2;
            // 
            // aGauge2
            // 
            this.aGauge2.BackColor = System.Drawing.SystemColors.Control;
            this.aGauge2.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge2.BaseArcRadius = 80;
            this.aGauge2.BaseArcStart = 135;
            this.aGauge2.BaseArcSweep = 270;
            this.aGauge2.BaseArcWidth = 2;
            this.aGauge2.Center = new System.Drawing.Point(100, 100);
            aGaugeLabel2.Color = System.Drawing.SystemColors.WindowText;
            aGaugeLabel2.Font = new System.Drawing.Font("Verdana", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            aGaugeLabel2.Name = "GaugeLabel2";
            aGaugeLabel2.Position = new System.Drawing.Point(85, 130);
            aGaugeLabel2.Text = "0";
            this.aGauge2.GaugeLabels.Add(aGaugeLabel2);
            aGaugeRange2.Color = System.Drawing.Color.Red;
            aGaugeRange2.EndValue = 200F;
            aGaugeRange2.InnerRadius = 70;
            aGaugeRange2.InRange = false;
            aGaugeRange2.Name = "AlertRange";
            aGaugeRange2.OuterRadius = 80;
            aGaugeRange2.StartValue = 160F;
            aGaugeRange3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            aGaugeRange3.EndValue = 160F;
            aGaugeRange3.InnerRadius = 70;
            aGaugeRange3.InRange = false;
            aGaugeRange3.Name = "GaugeRange3";
            aGaugeRange3.OuterRadius = 75;
            aGaugeRange3.StartValue = 0F;
            aGaugeRange4.Color = System.Drawing.Color.Lime;
            aGaugeRange4.EndValue = 160F;
            aGaugeRange4.InnerRadius = 75;
            aGaugeRange4.InRange = false;
            aGaugeRange4.Name = "GaugeRange2";
            aGaugeRange4.OuterRadius = 80;
            aGaugeRange4.StartValue = 0F;
            this.aGauge2.GaugeRanges.Add(aGaugeRange2);
            this.aGauge2.GaugeRanges.Add(aGaugeRange3);
            this.aGauge2.GaugeRanges.Add(aGaugeRange4);
            this.aGauge2.Location = new System.Drawing.Point(489, 12);
            this.aGauge2.MaxValue = 200F;
            this.aGauge2.MinValue = 0F;
            this.aGauge2.Name = "aGauge2";
            this.aGauge2.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Yellow;
            this.aGauge2.NeedleColor2 = System.Drawing.Color.Olive;
            this.aGauge2.NeedleRadius = 80;
            this.aGauge2.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.aGauge2.NeedleWidth = 2;
            this.aGauge2.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleLinesInterInnerRadius = 73;
            this.aGauge2.ScaleLinesInterOuterRadius = 80;
            this.aGauge2.ScaleLinesInterWidth = 1;
            this.aGauge2.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleLinesMajorInnerRadius = 70;
            this.aGauge2.ScaleLinesMajorOuterRadius = 80;
            this.aGauge2.ScaleLinesMajorStepValue = 20F;
            this.aGauge2.ScaleLinesMajorWidth = 2;
            this.aGauge2.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.aGauge2.ScaleLinesMinorInnerRadius = 75;
            this.aGauge2.ScaleLinesMinorOuterRadius = 80;
            this.aGauge2.ScaleLinesMinorTicks = 9;
            this.aGauge2.ScaleLinesMinorWidth = 1;
            this.aGauge2.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleNumbersFormat = null;
            this.aGauge2.ScaleNumbersRadius = 95;
            this.aGauge2.ScaleNumbersRotation = 0;
            this.aGauge2.ScaleNumbersStartScaleLine = 0;
            this.aGauge2.ScaleNumbersStepScaleLines = 1;
            this.aGauge2.Size = new System.Drawing.Size(205, 180);
            this.aGauge2.TabIndex = 3;
            this.aGauge2.Text = "aGauge2";
            this.aGauge2.Value = 0F;
            this.aGauge2.ValueChanged += new System.EventHandler(this.aGauge2_ValueChanged);
            // 
            // aGauge3
            // 
            this.aGauge3.BackColor = System.Drawing.SystemColors.Control;
            this.aGauge3.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge3.BaseArcRadius = 80;
            this.aGauge3.BaseArcStart = 135;
            this.aGauge3.BaseArcSweep = 270;
            this.aGauge3.BaseArcWidth = 2;
            this.aGauge3.Center = new System.Drawing.Point(100, 100);
            aGaugeLabel3.Color = System.Drawing.SystemColors.WindowText;
            aGaugeLabel3.Font = new System.Drawing.Font("Verdana", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            aGaugeLabel3.Name = "GaugeLabel3";
            aGaugeLabel3.Position = new System.Drawing.Point(85, 130);
            aGaugeLabel3.Text = "0";
            this.aGauge3.GaugeLabels.Add(aGaugeLabel3);
            aGaugeRange5.Color = System.Drawing.Color.Red;
            aGaugeRange5.EndValue = 100F;
            aGaugeRange5.InnerRadius = 70;
            aGaugeRange5.InRange = false;
            aGaugeRange5.Name = "AlertRange";
            aGaugeRange5.OuterRadius = 80;
            aGaugeRange5.StartValue = 80F;
            aGaugeRange6.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            aGaugeRange6.EndValue = 80F;
            aGaugeRange6.InnerRadius = 70;
            aGaugeRange6.InRange = false;
            aGaugeRange6.Name = "GaugeRange3";
            aGaugeRange6.OuterRadius = 75;
            aGaugeRange6.StartValue = 0F;
            aGaugeRange7.Color = System.Drawing.Color.Lime;
            aGaugeRange7.EndValue = 80F;
            aGaugeRange7.InnerRadius = 75;
            aGaugeRange7.InRange = false;
            aGaugeRange7.Name = "GaugeRange2";
            aGaugeRange7.OuterRadius = 80;
            aGaugeRange7.StartValue = 0F;
            this.aGauge3.GaugeRanges.Add(aGaugeRange5);
            this.aGauge3.GaugeRanges.Add(aGaugeRange6);
            this.aGauge3.GaugeRanges.Add(aGaugeRange7);
            this.aGauge3.Location = new System.Drawing.Point(54, 12);
            this.aGauge3.MaxValue = 100F;
            this.aGauge3.MinValue = 0F;
            this.aGauge3.Name = "aGauge3";
            this.aGauge3.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Yellow;
            this.aGauge3.NeedleColor2 = System.Drawing.Color.Olive;
            this.aGauge3.NeedleRadius = 80;
            this.aGauge3.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.aGauge3.NeedleWidth = 2;
            this.aGauge3.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge3.ScaleLinesInterInnerRadius = 73;
            this.aGauge3.ScaleLinesInterOuterRadius = 80;
            this.aGauge3.ScaleLinesInterWidth = 1;
            this.aGauge3.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge3.ScaleLinesMajorInnerRadius = 70;
            this.aGauge3.ScaleLinesMajorOuterRadius = 80;
            this.aGauge3.ScaleLinesMajorStepValue = 20F;
            this.aGauge3.ScaleLinesMajorWidth = 2;
            this.aGauge3.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.aGauge3.ScaleLinesMinorInnerRadius = 75;
            this.aGauge3.ScaleLinesMinorOuterRadius = 80;
            this.aGauge3.ScaleLinesMinorTicks = 9;
            this.aGauge3.ScaleLinesMinorWidth = 1;
            this.aGauge3.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge3.ScaleNumbersFormat = null;
            this.aGauge3.ScaleNumbersRadius = 95;
            this.aGauge3.ScaleNumbersRotation = 0;
            this.aGauge3.ScaleNumbersStartScaleLine = 0;
            this.aGauge3.ScaleNumbersStepScaleLines = 1;
            this.aGauge3.Size = new System.Drawing.Size(205, 180);
            this.aGauge3.TabIndex = 4;
            this.aGauge3.Text = "aGauge3";
            this.aGauge3.Value = 0F;
            this.aGauge3.ValueChanged += new System.EventHandler(this.aGauge3_ValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 273);
            this.Controls.Add(this.aGauge3);
            this.Controls.Add(this.aGauge2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.aGauge1);
            this.Name = "MainForm";
            this.Text = "C# WinCC OA Gauge Demo";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.AGauge aGauge1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.AGauge aGauge2;
        private System.Windows.Forms.AGauge aGauge3;
















    }
}

