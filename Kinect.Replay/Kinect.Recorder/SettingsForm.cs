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
        public int windowSize, mirThresh, swayThresh, leanThresh, hingeThresh, svlThresh, bpmvThresh, bpmdThresh;
        public SettingsForm(int wiSize, int mT, int sT, int lT, int hT, int svlT, int bpmvT, int bpmdT)
        {
            InitializeComponent();
            windowSize = wiSize;
            svlWS.Value = windowSize;
            mirThresh = mT;
            mirThr.Value = mirThresh;
            swayThresh = sT;
            swaThr.Value = swayThresh;
            leanThresh = lT;
            leaThr.Value = leanThresh;
            hingeThresh = hT;
            hinThr.Value = hingeThresh;
            svlThresh = svlT;
            svlThr.Value = svlThresh;
            bpmvThresh = bpmvT;
            bpmvThr.Value = bpmvThresh;
            bpmdThresh = bpmdT;
            bpmdThr.Value = bpmdThresh;
        }

        private void svlWS_ValueChanged(object sender, EventArgs e)
        {
            windowSize = svlWS.Value;
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mirThr_ValueChanged(object sender, EventArgs e)
        {
            mirThresh = mirThr.Value;
        }

        private void swaThr_ValueChanged(object sender, EventArgs e)
        {
            swayThresh = swaThr.Value;
        }

        private void leaThr_ValueChanged(object sender, EventArgs e)
        {
            leanThresh = leaThr.Value;
        }

        private void hinThr_ValueChanged(object sender, EventArgs e)
        {
            hingeThresh = hinThr.Value;
        }

        private void svlThr_ValueChanged(object sender, EventArgs e)
        {
            svlThresh = svlThr.Value;
        }

        private void bpmvThr_ValueChanged(object sender, EventArgs e)
        {
            bpmvThresh = bpmvThr.Value;
        }

        private void bpmdThr_ValueChanged(object sender, EventArgs e)
        {
            bpmdThresh = bpmdThr.Value;
        }

        private void reset_Click(object sender, EventArgs e)
        {
            windowSize = 100;
            svlWS.Value = 100;
            mirThresh = 100;
            mirThr.Value = 100;
            swayThresh = 100;
            swaThr.Value = 100;
            leanThresh = 100;
            leaThr.Value = 100;
            hingeThresh = 100;
            hinThr.Value = 100;
            svlThresh = 100;
            svlThr.Value = 100;
            bpmvThresh = 100;
            bpmvThr.Value = 100;
            bpmdThresh = 100;
            bpmdThr.Value = 100;
        }

    }
}
