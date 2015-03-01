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
            this.SvLWS = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.MTBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.STBox = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.LTBox = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.HTBox = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.SVLTBox = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.BPMTBox = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.SvLWS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MTBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.STBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LTBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HTBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SVLTBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BPMTBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SvLWS
            // 
            this.SvLWS.Location = new System.Drawing.Point(57, 55);
            this.SvLWS.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.SvLWS.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.SvLWS.Name = "SvLWS";
            this.SvLWS.Size = new System.Drawing.Size(120, 20);
            this.SvLWS.TabIndex = 0;
            this.SvLWS.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.SvLWS.ValueChanged += new System.EventHandler(this.SvLWS_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Staccato vs Legato Window Size";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(57, 323);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.close_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mirroring Threshold";
            // 
            // MTBox
            // 
            this.MTBox.DecimalPlaces = 2;
            this.MTBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.MTBox.Location = new System.Drawing.Point(57, 135);
            this.MTBox.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            65536});
            this.MTBox.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.MTBox.Name = "MTBox";
            this.MTBox.Size = new System.Drawing.Size(120, 20);
            this.MTBox.TabIndex = 3;
            this.MTBox.Value = new decimal(new int[] {
            19,
            0,
            0,
            131072});
            this.MTBox.ValueChanged += new System.EventHandler(this.MTBox_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(67, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Swaying Threshold";
            // 
            // STBox
            // 
            this.STBox.DecimalPlaces = 2;
            this.STBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.STBox.Location = new System.Drawing.Point(57, 174);
            this.STBox.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.STBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.STBox.Name = "STBox";
            this.STBox.Size = new System.Drawing.Size(120, 20);
            this.STBox.TabIndex = 5;
            this.STBox.Value = new decimal(new int[] {
            2,
            0,
            0,
            131072});
            this.STBox.ValueChanged += new System.EventHandler(this.STBox_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(67, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Leaning Threshold";
            // 
            // LTBox
            // 
            this.LTBox.DecimalPlaces = 2;
            this.LTBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.LTBox.Location = new System.Drawing.Point(57, 213);
            this.LTBox.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.LTBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.LTBox.Name = "LTBox";
            this.LTBox.Size = new System.Drawing.Size(120, 20);
            this.LTBox.TabIndex = 7;
            this.LTBox.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.LTBox.ValueChanged += new System.EventHandler(this.LTBox_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(58, 236);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Hinge Check Threshold";
            // 
            // HTBox
            // 
            this.HTBox.DecimalPlaces = 2;
            this.HTBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.HTBox.Location = new System.Drawing.Point(57, 252);
            this.HTBox.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            this.HTBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.HTBox.Name = "HTBox";
            this.HTBox.Size = new System.Drawing.Size(120, 20);
            this.HTBox.TabIndex = 9;
            this.HTBox.Value = new decimal(new int[] {
            8,
            0,
            0,
            131072});
            this.HTBox.ValueChanged += new System.EventHandler(this.HTBox_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(151, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Staccato Vs Legato Threshold";
            // 
            // SVLTBox
            // 
            this.SVLTBox.DecimalPlaces = 2;
            this.SVLTBox.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.SVLTBox.Location = new System.Drawing.Point(57, 96);
            this.SVLTBox.Maximum = new decimal(new int[] {
            34,
            0,
            0,
            65536});
            this.SVLTBox.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.SVLTBox.Name = "SVLTBox";
            this.SVLTBox.Size = new System.Drawing.Size(120, 20);
            this.SVLTBox.TabIndex = 11;
            this.SVLTBox.Value = new decimal(new int[] {
            17,
            0,
            0,
            65536});
            this.SVLTBox.ValueChanged += new System.EventHandler(this.SVLTBox_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(82, 275);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "BPM Threshold";
            // 
            // BPMTBox
            // 
            this.BPMTBox.DecimalPlaces = 3;
            this.BPMTBox.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.BPMTBox.Location = new System.Drawing.Point(57, 291);
            this.BPMTBox.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.BPMTBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.BPMTBox.Name = "BPMTBox";
            this.BPMTBox.Size = new System.Drawing.Size(120, 20);
            this.BPMTBox.TabIndex = 13;
            this.BPMTBox.Value = new decimal(new int[] {
            25,
            0,
            0,
            196608});
            this.BPMTBox.ValueChanged += new System.EventHandler(this.BPMTBox_ValueChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 358);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.BPMTBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.SVLTBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.HTBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.LTBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.STBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MTBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SvLWS);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.SvLWS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MTBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.STBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LTBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HTBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SVLTBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BPMTBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown SvLWS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown MTBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown STBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown LTBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown HTBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown SVLTBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown BPMTBox;
    }
}