using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kinect.Recorder
{
    public partial class SettingsForm : Form
    {
        public int windowSize;
        public float mirThresh, swayThresh, leanThresh, hingeThresh, svlThresh, bpmThresh;
        public SettingsForm(int wiSize, float mT, float sT, float lT, float hT, float svlT, float bpmT)
        {
            InitializeComponent();
            windowSize = wiSize;
            SvLWS.Value = windowSize;
            mirThresh = mT;
            MTBox.Value = Convert.ToDecimal(mirThresh);
            swayThresh = sT;
            STBox.Value = Convert.ToDecimal(swayThresh);
            leanThresh = lT;
            LTBox.Value = Convert.ToDecimal(leanThresh);
            hingeThresh = hT;
            HTBox.Value = Convert.ToDecimal(hingeThresh);
            svlThresh = svlT;
            SVLTBox.Value = Convert.ToDecimal(svlThresh);
            bpmThresh = bpmT;
            BPMTBox.Value = Convert.ToDecimal(bpmThresh);
        }

        private void SvLWS_ValueChanged(object sender, EventArgs e)
        {
            windowSize = Convert.ToInt32(SvLWS.Value);
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MTBox_ValueChanged(object sender, EventArgs e)
        {
            mirThresh = (float)Convert.ToDouble(MTBox.Value);
        }

        private void STBox_ValueChanged(object sender, EventArgs e)
        {
            swayThresh = (float)Convert.ToDouble(STBox.Value);
        }

        private void LTBox_ValueChanged(object sender, EventArgs e)
        {
            leanThresh = (float)Convert.ToDouble(LTBox.Value);
        }

        private void HTBox_ValueChanged(object sender, EventArgs e)
        {
            hingeThresh = (float)Convert.ToDouble(HTBox.Value);
        }

        private void SVLTBox_ValueChanged(object sender, EventArgs e)
        {
            svlThresh = (float)Convert.ToDouble(SVLTBox.Value);
        }

        private void BPMTBox_ValueChanged(object sender, EventArgs e)
        {
            bpmThresh = (float)Convert.ToDouble(BPMTBox.Value);
        }


    }
}
