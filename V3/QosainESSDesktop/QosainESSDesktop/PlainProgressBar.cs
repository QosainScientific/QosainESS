using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QosainESSDesktop
{
    public partial class PlainProgressBar : UserControl
    {
        DateTime started = new DateTime();
        DateTime updated = new DateTime();

        double speed = .00000001;
        List<double> estimates = new List<double>();
        double valueAtUpdate = 0;
        object pausedAt;
        TimeSpan timeInPauses = new TimeSpan();
        public PlainProgressBar()
        {
            InitializeComponent();
        }
        public void Started()
        {
            Reset();
            timeInPauses = new TimeSpan();
            pausedAt = null;
            Visible = true;
            started = DateTime.Now;
            progressBar1.Value = 0;
            progressL.Text = "0";

            progressBar1.Visible = true;
            progressL.Visible = true;
            progressL.Visible = true;
            percentL.Visible = true;
            valueAtUpdate = 0;
            this.timer1.Enabled = true;
        }
        public double Value
        {
            set
            {
                try
                {
                    if (progressBar1.Value == value)
                        return;
                    if (!timer1.Enabled)
                        return;
                    if (value == 0) // its a reset
                        progressBar1.Value = 0;
                    var elapsed = DateTime.Now - started;
                    speed = value / elapsed.TotalSeconds;
                    valueAtUpdate = value;
                    updated = DateTime.Now;
                    if (value >= 100)
                        ;
                }
                catch { }
            }
        }

        public void Ended()
        {
            Visible = false;
            this.timer1.Enabled = false;
        }
        private void PlainProgressBar_Load(object sender, EventArgs e)
        {

        }
        public void Reset()
        {
            started = new DateTime();
            updated = new DateTime();
            speed = .00000001;
            timeInPauses = new TimeSpan();
            pausedAt = null;
            estimates.Clear();
            Value = 0;
            valueAtUpdate = 0;
        }
        public void ForceTimeEstimate(double time)
        {
            Value = 0;
            speed = 100 / time;
            valueAtUpdate = 0;
        }
        public void Pause()
        {
            if (pausedAt == null)
                pausedAt = DateTime.Now;
        }
        public void Resume()
        {
            if (pausedAt == null)
                return;
            timeInPauses += DateTime.Now - (DateTime)pausedAt;
            pausedAt = null;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (speed == 0)
                speed = .0000001;
            if (pausedAt != null) 
                return;
            var now = DateTime.Now;
            var elapsed = now - started - timeInPauses;
            double projectedValue = valueAtUpdate + speed * (now - updated).TotalSeconds;
            double secondsRemaining = (100 - projectedValue) / speed;
            if (this.Visible)
                ;
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
            else if (secondsRemaining > 24 * 60 * 60 * 2)
            {
                elapsedL.Text = "--";
                remainingL.Text = "estimating time remaining...";
                startedL.Text = "--";
                progressBar1.Visible = false;
                progressL.Visible = false;
                percentL.Visible = false;
                return;
            }
            if (progressBar1.Value < projectedValue || estimates.Count <= 1)
                progressBar1.Value = (int)Math.Round(projectedValue);
            progressL.Text = progressBar1.Value.ToString();
            estimates.Add(secondsRemaining);
            if (estimates.Count == 1)
            {
                return;
            }
            if (estimates.Count > 100)
                estimates.RemoveAt(0);
            if (estimates.Count < 100 && valueAtUpdate < 2)
            {
                elapsedL.Text = "--";
                remainingL.Text = "estimating time remaining...";
                startedL.Text = "--";
                progressBar1.Visible = false;
                progressL.Visible = false;
                percentL.Visible = false;
                return;
            }
            progressBar1.Visible = true;
            progressL.Visible = true;
            percentL.Visible = true;

            elapsedL.Text = elapsed.ToString(@"hh\:mm\:ss");
            startedL.Text = started.ToLongTimeString();
            remainingL.Text = TimeString(estimates.Average());
        }

        public static string TimeString(double sec)
        {
            double accuracyPC = 5;
            double secondsRoundD = sec * accuracyPC / 100;
            double minutesToRoundD = sec * accuracyPC / 100 / 60;
            int secondsToRound = 5;
            if (secondsRoundD > 30)
                secondsToRound = 30;
            else if (secondsRoundD > 20)
                secondsToRound = 20;
            else if (secondsRoundD > 15)
                secondsToRound = 15;
            else if (secondsRoundD > 10)
                secondsToRound = 10;
            int minutesToRound = 1;
            if (minutesToRoundD > 60)
                minutesToRound = 60;
            else if (minutesToRoundD > 30)
                minutesToRound = 30;
            else if (minutesToRoundD > 20)
                minutesToRound = 20;
            else if (minutesToRoundD > 15)
                minutesToRound = 15;
            else if (minutesToRoundD > 10)
                minutesToRound = 10;
            else if (minutesToRoundD > 5)
                minutesToRound = 5;
            int seconds = ((((int)Math.Round(sec)) % 60) / secondsToRound) * secondsToRound;
            int minutes = (((((int)sec) % 3600) / 60) / minutesToRound) * minutesToRound;
            int hours = ((int)sec) / 3600;

            string hoursStr = "", minutesStr = "", secondsStr = "";
            if (hours > 0)
                hoursStr = hours + " hour" + (hours > 1 ? "s" : "");
            if (minutes > 0)
                minutesStr = minutes + " minute" + (minutes > 1 ? "s" : "");
            if (sec < 180)
                ;
            if (seconds > 0)
            {
                if (hours <= 0 && minutesToRound < 60)
                    secondsStr = seconds + " second" + (seconds > 1 ? "s" : "");
            }
            var parts = new string[] { hoursStr, minutesStr, secondsStr }.ToList().FindAll(p => p != "");

            string ans = "";
            for (int i = 0; i < parts.Count; i++)
            {
                if (i == 1) // , or and
                {
                    if (parts.Count == 2)
                        ans += " and ";
                    else
                        ans += ", ";
                }
                if (i == 2) // , or and
                {
                    if (parts.Count > 1)
                        ans += " and ";
                }
                ans += parts[i];
            }

            if (ans.Length == 0)
                ans = "Almost there...";
            return ans;
        }
    }
}
