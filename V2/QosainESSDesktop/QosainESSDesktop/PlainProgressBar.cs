using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QosainESSDesktop
{
    public partial class PlainProgressBar : UserControl
    {
        public PlainProgressBar()
        {
            InitializeComponent();
        }
        DateTime started = new DateTime();
        DateTime updated = new DateTime();
        public void Started()
        {
            Visible = true;
            started = DateTime.Now;
            progressBar1.Value = 0;
            progressL.Text = "0";

            progressBar1.Visible = true;
            progressL.Visible = true;
            progressL.Visible = true;
            percentL.Visible = true;
            valueAtUpdate = 0;


        }
        double valueAtUpdate = 0;
        public double Value
        {
            set
            {
                try
                {
                    if (progressBar1.Value == (int)value)
                        return;
                    progressBar1.Value = (int)value; 
                    progressL.Text = value.ToString();
                    var elapsed = DateTime.Now - started;
                    speed = progressBar1.Value / elapsed.TotalSeconds;
                    valueAtUpdate = progressBar1.Value;
                    updated = DateTime.Now;
                }
                catch { }
            }
        }

        public void Ended()
        { Visible = false; }
        private void PlainProgressBar_Load(object sender, EventArgs e)
        {

        }

        double speed = .00000001;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (speed == 0)
                speed = .0000001;
            var elapsed = DateTime.Now - started;
            double projectedValue = valueAtUpdate + speed * (DateTime.Now - updated).TotalSeconds;
            double secondsRemaining = (100 - projectedValue) / speed;
            if (secondsRemaining <= 0)
            {
                elapsedL.Text = "--";
                remainingL.Text = "Almost there...";
                startedL.Text = "--";
                progressBar1.Visible = false;
                progressL.Visible = false;
                percentL.Visible = false;
                return;
            }
            else if (secondsRemaining > 24 * 60 * 60)
            {
                elapsedL.Text = "--";
                remainingL.Text = "estimating time remaining...";
                startedL.Text = "--";
                progressBar1.Visible = false;
                progressL.Visible = false;
                percentL.Visible = false;
                return;
            }
            else
            {
                progressBar1.Visible = true;
                progressL.Visible = true;
                percentL.Visible = true;
            }
            elapsedL.Text = elapsed.ToString(@"hh\:mm\:ss");
            startedL.Text = started.ToLongTimeString();
            remainingL.Text = new TimeSpan(0, 0, 0, (int)secondsRemaining).ToString(@"hh\:mm\:ss");
        }
    }
}
