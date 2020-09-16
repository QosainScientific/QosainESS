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
        public void Started()
        {                      
            Visible = true;
            started = DateTime.Now;
            progressBar1.Value = 0;
            progressL.Text = "0";
            lastSpeed = 0;
            progressBar1.Visible = true;
            progressL.Visible = true;    
            progressL.Visible = true;
            percentL.Visible = true;


        }      
        public double Value { set { try { progressBar1.Value = (int)value; progressL.Text = value.ToString(); } catch { } } }  

        public void Ended()
        { Visible = false; }
        private void PlainProgressBar_Load(object sender, EventArgs e)
        {

        }

        double lastSpeed = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - started;
            double fac = 0.99;
            double speed = lastSpeed * fac + progressBar1.Value / elapsed.TotalSeconds * (1 - fac);
            double secondsRemaining = (100 - progressBar1.Value) / speed;
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
            elapsedL.Text = elapsed.ToString(@"hh\:mm\:ss");
            startedL.Text = started.ToLongTimeString();
            remainingL.Text = new TimeSpan(0,0,0,(int)secondsRemaining).ToString(@"hh\:mm\:ss");
            lastSpeed = speed;
        }
    }
}
