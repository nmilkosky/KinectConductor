namespace Kinect.Recorder
{
    partial class SettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.svlWS = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.svlThr = new System.Windows.Forms.TrackBar();
            this.mirThr = new System.Windows.Forms.TrackBar();
            this.leaThr = new System.Windows.Forms.TrackBar();
            this.bpmvThr = new System.Windows.Forms.TrackBar();
            this.swaThr = new System.Windows.Forms.TrackBar();
            this.hinThr = new System.Windows.Forms.TrackBar();
            this.bpmdThr = new System.Windows.Forms.TrackBar();
            this.resetButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.svlWS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svlThr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mirThr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leaThr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bpmvThr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.swaThr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hinThr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bpmdThr)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Staccato vs Legato History Size";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(221, 298);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(120, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.close_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mirroring Threshold";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(232, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Swaying Threshold";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Leaning Threshold";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(222, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Hinge Check Threshold";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(205, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(151, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Staccato Vs Legato Threshold";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(31, 231);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "BPM Velocity Threshold";
            // 
            // svlWS
            // 
            this.svlWS.LargeChange = 50;
            this.svlWS.Location = new System.Drawing.Point(31, 55);
            this.svlWS.Maximum = 150;
            this.svlWS.Minimum = 50;
            this.svlWS.Name = "svlWS";
            this.svlWS.Size = new System.Drawing.Size(120, 45);
            this.svlWS.SmallChange = 25;
            this.svlWS.TabIndex = 15;
            this.svlWS.TickFrequency = 25;
            this.svlWS.Value = 100;
            this.svlWS.ValueChanged += new System.EventHandler(this.svlWS_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(222, 231);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "BPM Distance Threshold";
            // 
            // svlThr
            // 
            this.svlThr.LargeChange = 50;
            this.svlThr.Location = new System.Drawing.Point(221, 55);
            this.svlThr.Maximum = 150;
            this.svlThr.Minimum = 50;
            this.svlThr.Name = "svlThr";
            this.svlThr.Size = new System.Drawing.Size(120, 45);
            this.svlThr.SmallChange = 25;
            this.svlThr.TabIndex = 23;
            this.svlThr.TickFrequency = 25;
            this.svlThr.Value = 100;
            this.svlThr.ValueChanged += new System.EventHandler(this.svlThr_ValueChanged);
            // 
            // mirThr
            // 
            this.mirThr.LargeChange = 50;
            this.mirThr.Location = new System.Drawing.Point(31, 119);
            this.mirThr.Maximum = 150;
            this.mirThr.Minimum = 50;
            this.mirThr.Name = "mirThr";
            this.mirThr.Size = new System.Drawing.Size(120, 45);
            this.mirThr.SmallChange = 25;
            this.mirThr.TabIndex = 24;
            this.mirThr.TickFrequency = 25;
            this.mirThr.Value = 100;
            this.mirThr.ValueChanged += new System.EventHandler(this.mirThr_ValueChanged);
            // 
            // leaThr
            // 
            this.leaThr.LargeChange = 50;
            this.leaThr.Location = new System.Drawing.Point(31, 183);
            this.leaThr.Maximum = 150;
            this.leaThr.Minimum = 50;
            this.leaThr.Name = "leaThr";
            this.leaThr.Size = new System.Drawing.Size(120, 45);
            this.leaThr.SmallChange = 25;
            this.leaThr.TabIndex = 25;
            this.leaThr.TickFrequency = 25;
            this.leaThr.Value = 100;
            this.leaThr.ValueChanged += new System.EventHandler(this.leaThr_ValueChanged);
            // 
            // bpmvThr
            // 
            this.bpmvThr.LargeChange = 50;
            this.bpmvThr.Location = new System.Drawing.Point(31, 247);
            this.bpmvThr.Maximum = 150;
            this.bpmvThr.Minimum = 50;
            this.bpmvThr.Name = "bpmvThr";
            this.bpmvThr.Size = new System.Drawing.Size(120, 45);
            this.bpmvThr.SmallChange = 25;
            this.bpmvThr.TabIndex = 26;
            this.bpmvThr.TickFrequency = 25;
            this.bpmvThr.Value = 100;
            this.bpmvThr.ValueChanged += new System.EventHandler(this.bpmvThr_ValueChanged);
            // 
            // swaThr
            // 
            this.swaThr.LargeChange = 50;
            this.swaThr.Location = new System.Drawing.Point(221, 119);
            this.swaThr.Maximum = 150;
            this.swaThr.Minimum = 50;
            this.swaThr.Name = "swaThr";
            this.swaThr.Size = new System.Drawing.Size(120, 45);
            this.swaThr.SmallChange = 25;
            this.swaThr.TabIndex = 27;
            this.swaThr.TickFrequency = 25;
            this.swaThr.Value = 100;
            this.swaThr.ValueChanged += new System.EventHandler(this.swaThr_ValueChanged);
            // 
            // hinThr
            // 
            this.hinThr.LargeChange = 50;
            this.hinThr.Location = new System.Drawing.Point(221, 183);
            this.hinThr.Maximum = 150;
            this.hinThr.Minimum = 50;
            this.hinThr.Name = "hinThr";
            this.hinThr.Size = new System.Drawing.Size(120, 45);
            this.hinThr.SmallChange = 25;
            this.hinThr.TabIndex = 28;
            this.hinThr.TickFrequency = 25;
            this.hinThr.Value = 100;
            this.hinThr.ValueChanged += new System.EventHandler(this.hinThr_ValueChanged);
            // 
            // bpmdThr
            // 
            this.bpmdThr.LargeChange = 50;
            this.bpmdThr.Location = new System.Drawing.Point(221, 247);
            this.bpmdThr.Maximum = 150;
            this.bpmdThr.Minimum = 50;
            this.bpmdThr.Name = "bpmdThr";
            this.bpmdThr.Size = new System.Drawing.Size(120, 45);
            this.bpmdThr.SmallChange = 25;
            this.bpmdThr.TabIndex = 29;
            this.bpmdThr.TickFrequency = 25;
            this.bpmdThr.Value = 100;
            this.bpmdThr.ValueChanged += new System.EventHandler(this.bpmdThr_ValueChanged);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(31, 298);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(120, 23);
            this.resetButton.TabIndex = 30;
            this.resetButton.Text = "Reset to Default";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.reset_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 325);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.bpmdThr);
            this.Controls.Add(this.hinThr);
            this.Controls.Add(this.swaThr);
            this.Controls.Add(this.bpmvThr);
            this.Controls.Add(this.leaThr);
            this.Controls.Add(this.mirThr);
            this.Controls.Add(this.svlThr);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.svlWS);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label1);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.svlWS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svlThr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mirThr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leaThr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bpmvThr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.swaThr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hinThr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bpmdThr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar svlWS;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar svlThr;
        private System.Windows.Forms.TrackBar mirThr;
        private System.Windows.Forms.TrackBar leaThr;
        private System.Windows.Forms.TrackBar bpmvThr;
        private System.Windows.Forms.TrackBar swaThr;
        private System.Windows.Forms.TrackBar hinThr;
        private System.Windows.Forms.TrackBar bpmdThr;
        private System.Windows.Forms.Button resetButton;
    }
}