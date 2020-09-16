using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using FivePointNine.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPRGUI2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();                 
        }          

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        
        private void closeB_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void minimizeB_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void windowedModeB_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else
                WindowState = FormWindowState.Maximized;
        }


        void SendCom(string com)
        {
            if (dataPort.Channel == null)
            {
                MessageBox.Show("Kindly connect to the hardware first");
                return;
            }
            dataPort.Channel.Write(new UTF8Encoding().GetBytes(com + "\n"), 0, com.Length + 1);
        }
        string waitForResponse(string com, int timeOut)
        {
            var start = DateTime.Now;
            var rec = "";
            while (getCommand(rec) != com && timeOut > 0)
            {
                rec = ReceiveCom(timeOut);
                timeOut -= (int)(DateTime.Now - start).TotalMilliseconds;
                start = DateTime.Now;
            }
            if (getCommand(rec) == com)
                return rec;
            else
                return "";
        }
        string ReceiveCom(int timeOut = 1000)
        {
            if (dataPort.Channel == null)
                return "";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < timeOut; i+=30)
            {
                if (dataPort.Channel == null)
                    return "";
                while (dataPort.Channel.BytesToRead > 0)
                {
                    char c = (char)dataPort.Channel.ReadByte();
                    if (c == '\r')
                        continue;
                    if (c == '\n')
                    {
                        if (sb.ToString() == ", xy = idle")
                            ;
                        return sb.ToString();
                    }
                    sb.Append(c.ToString());
                    i = 0;
                }
                System.Threading.Thread.Sleep(30);
                Application.DoEvents();
            }
            return sb.ToString();
        }
        Timer poller;
        private void dataPort_Connected(object sender, EventArgs e)
        {
            var resp = waitForResponse("begin", 5000);
            if (resp != "begin")
            {
                dataPort.Disconnect();
                MessageBox.Show("Could not connect to the hardware");
                return;
            }
            SendCom("home all");
            poller = new Timer();
            poller.Interval = 200;
            poller.Tick += Poller_Tick;
            poller.Enabled = true;
        }

        bool inTick = false;
        private void Poller_Tick(object sender, EventArgs e)
        {
            if (inTick)
                return;
            inTick = true;
            SendCom("status");
            while (dataPort.Channel.BytesToRead > 0)
            {
                var raw = ReceiveCom(500);
                var name = getCommand(raw);
                var args = getArgs(raw);
                if (name == "status")
                {
                    xCoordL.Text = args["x"];
                    yCoordL.Text = args["y"];
                    rasterView1.UpdateViewXY(Convert.ToSingle(args["x"]), Convert.ToSingle(args["y"]));
                    xyStageStatusL.Text = args["xy stage"];
                    pumpStatusL.Text = args["pump"];
                    if (xyStageStatusL.Text == "Moving" || xyStageStatusL.Text == "Raster")
                        xyStageStatusL.BackColor = Color.FromArgb(255, 128, 128);
                    else if (xyStageStatusL.Text == "Homing")
                    {
                        xyStageStatusL.BackColor = Color.FromArgb(128, 255, 128);
                        xCoordL.Text = "Homing";
                        yCoordL.Text = "Homing";
                    }
                    else if (xyStageStatusL.Text == "Idle")
                        xyStageStatusL.BackColor = Color.DimGray;
                    if (pumpStatusL.Text == "Pumping")
                        pumpStatusL.BackColor = Color.FromArgb(255, 128, 128);
                    else if (pumpStatusL.Text == "Homing")
                        pumpStatusL.BackColor = Color.FromArgb(128, 255, 128);
                    else if (pumpStatusL.Text == "Idle")
                        pumpStatusL.BackColor = Color.DimGray;
                    Application.DoEvents();
                }
                else if (name == "info")
                {
                    if (args["tag"] == "homing")
                        ;
                    else
                    {
                        var msg = args["message"];
                    }
                }
                else if (name == "pump end" || name == "pump error")
                {
                    MessageBox.Show(args["message"], "", MessageBoxButtons.OK, name == "pump error" ? MessageBoxIcon.Error : MessageBoxIcon.Information);
                    pumpBeginB.Text = "Begin";
                }
                else if (name == "raster end")
                {
                    MessageBox.Show(args["message"], "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    rasterBeginB.Text = "Begin";
                }
                else if (name != "")
                {
                }
            }
            inTick = false;
        }

        private void dataPort_Disconnected(object sender, EventArgs e)
        {
            poller.Enabled = false;
            poller = null;
        }
             

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {                     
        }                      
        private void dataPort_DevicesRefreshed(object sender, EventArgs e)
        {
            dataPort.AddAddress("Virtual PhysLogger 1.0");
        }
                            
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;
        
        protected override void WndProc(ref Message m)
        {
            if (WindowState == FormWindowState.Normal)
            {
                if (m.Msg == 0x84)
                {  // Trap WM_NCHITTEST
                    Point pos = new Point(m.LParam.ToInt32());
                    pos = this.PointToClient(pos);
                    if (pos.Y < cCaption)
                    {
                        m.Result = (IntPtr)2;  // HTCAPTION
                        return;
                    }
                    if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                    {
                        m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                        return;
                    }
                }
            }
            base.WndProc(ref m);
        }
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
        }


        Point lastDragPoint = new Point();
        private void label8_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragginWindows)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    int wid = Width, hei = Height;
                    WindowState = FormWindowState.Normal;
                    Top = 0;
                    Width = wid - 100;
                    Height = hei - 100;
                    Left = lastDragPoint.X - Width / 2;
                    lastDragPoint = new Point(Width / 2, lastDragPoint.Y);
                }
                else if (WindowState == FormWindowState.Normal)
                {
                    if (Top + lastDragPoint.Y < 1)
                    {
                        WindowState = FormWindowState.Maximized;
                        isDragginWindows = false;
                    }
                }
                Left += e.X - lastDragPoint.X;
                Top += e.Y - lastDragPoint.Y;
            }
            else
                lastDragPoint = e.Location;
        }

        bool isDragginWindows = false;
        private void label8_MouseDown(object sender, MouseEventArgs e)
        {
            isDragginWindows = true;
        }

        private void label8_MouseUp(object sender, MouseEventArgs e)
        {
            isDragginWindows = false;
        }

        private void label8_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                WindowState = FormWindowState.Maximized;
            else
                WindowState = FormWindowState.Normal;
        }

        private void _8DirectionButtonSet1_OnButtonClick(int id)
        {
            if (id < 0 || id > 8)
                return;
            string [] comTable = { "y+", "y++", "y-", "y--", "x-", "x--", "x+", "x++"};
            SendCom(comTable[id]);
        }

        private void RasterParamTB_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                rasterView1.UpdateViewRaster(Convert.ToSingle(rasterWidthTB.Text), Convert.ToSingle(rasterHeightTB.Text), Convert.ToSingle(rasterStepSizeTB.Text));

                float dist = 0;
                float rWidth = Convert.ToSingle(rasterWidthTB.Text);
                float rHeight = Convert.ToSingle(rasterHeightTB.Text);
                float step= Convert.ToSingle(rasterStepSizeTB.Text);
                float speed = Convert.ToSingle(rasterSpeedTB.Text);
                for (float y = 0; y < rHeight; y += step)
                {
                    dist += rWidth;
                    if (y + step <= rHeight)
                        dist += (float)Math.Sqrt(rWidth * rWidth + step * step);
                }
                

            }
            catch { rasterView1.HideRaster(); }
        }
        private void spAutoB_Click(object sender, EventArgs e)
        {
            int inc = 200;
            for (int i = spModesSC.SplitterDistance; i < spModesSC.Height - spManualB.Top - spManualB.Height; i+=inc)
            {
                System.Threading.Thread.Sleep(20);
                spModesSC.SplitterDistance = i;
                Application.DoEvents();
                inc = (int)Math.Round(inc * 0.75F) + 1;
            }
            spModesSC.SplitterDistance = spModesSC.Height - spManualB.Top - spManualB.Height;
        }

        public string getCommand(string CommandRaw)
        {
            if (CommandRaw == "")
                return "";
            return CommandRaw.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
        }
        public Dictionary<string, string> getArgs(string CommandRaw)
        {
            var comParts = CommandRaw.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (comParts.Length == 2)
                comParts = comParts[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            else
                comParts = new string[0];
            Dictionary<string, string> Args = new Dictionary<string, string>();
            for (int i = 0; i < comParts.Length; i++)
            {
                var pair = comParts[i].Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                Args.Add(pair[0].Trim(), pair[1].Trim());
            }
            return Args;
        }
        private void spManualB_Click(object sender, EventArgs e)
        {
            int inc = 200;
            for (int i = spModesSC.SplitterDistance; i >= spAutoB.Top + spAutoB.Height; i -= inc)
            {

                System.Threading.Thread.Sleep(20);
                spModesSC.SplitterDistance = i;
                Application.DoEvents();
                inc = (int)Math.Round(inc * 0.75F) + 1;
            }
            spModesSC.SplitterDistance = spAutoB.Top + spAutoB.Height;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        bool inPumpStart = false;
        private void pumpBeginB_Click(object sender, EventArgs e)
        {
            if (inPumpStart)
                return;
            inPumpStart = true;
            while (inTick)
            {
                System.Threading.Thread.Sleep(30);
                Application.DoEvents();
            }
            inTick = true;
            if (((Button)sender).Text == "Begin")
            {
                try
                {
                    SendCom("pump start: " +
                        "diameter = " + Convert.ToDouble(syringeDiaTB.Text) +
                        ", rate = " + Convert.ToDouble(syringeFlowRateTB.Text) / 3600 / (Convert.ToDouble(syringeDiaTB.Text) * Math.PI) +
                        ", max time = " + (enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Text) * 60000 : -1) +
                        ", max dist = " + (enableVolumeLimitB.Checked ? Convert.ToDouble(syringeVolumeLimitTB.Text) / (Convert.ToDouble(syringeDiaTB.Text) * Math.PI) : -1)
                        );
                    var resp = waitForResponse("pump resp", 2000);
                    if (resp == "")
                        MessageBox.Show("The pump could not be started");
                    else
                    {
                        if (getArgs(resp)["answer"] == "no")
                            MessageBox.Show(getArgs(resp)["message"]);
                        else
                        {
                            ((Button)sender).Text = "Stop";
                        }
                    }
                }
                catch { }
            }
            else
            {
                SendCom("pump stop");
                var resp = waitForResponse("pump stop resp", 2000);
                if (resp != "")
                {
                    if (getArgs(resp)["answer"] == "stopped")
                        ((Button)sender).Text = "Begin";
                }
            }
            inTick = false;
            inPumpStart = false;
        }

        private void spUpB_Click(object sender, EventArgs e)
        {
            SendCom("z+");
            var resp = waitForResponse("z moving", 1000);
        }

        private void spUpUpB_Click(object sender, EventArgs e)
        {

            SendCom("z++");
        }

        private void spDownB_Click(object sender, EventArgs e)
        {

            SendCom("z-");
        }

        private void spDownDownB_Click(object sender, EventArgs e)
        {

            SendCom("z--");
        }

        private void spTopB_Click(object sender, EventArgs e)
        {   
            SendCom("ztop");
        }

        bool inRasterStart = false;
        private void rasterBeginB_Click(object sender, EventArgs e)
        {
            if (inRasterStart)
                return;
            inRasterStart = true;
            while (inTick)
            {
                System.Threading.Thread.Sleep(30);
                Application.DoEvents();
            }
            inTick = true;
            if (((Button)sender).Text == "Begin")
            {
                try
                {
                    if (Convert.ToDouble(rasterWidthTB.Text) <= 0)
                        throw new Exception();
                    if (Convert.ToDouble(rasterHeightTB.Text) <= 0)
                        throw new Exception();
                    if (Convert.ToDouble(rasterStepSizeTB.Text) <= 0)
                        throw new Exception();
                    if (Convert.ToDouble(rasterSpeedTB.Text) <= 0 || Convert.ToDouble(rasterSpeedTB.Text) > 5)
                        throw new Exception();
                    SendCom("raster: mode = 1" +
                        ", lenX = " + Convert.ToDouble(rasterWidthTB.Text) +
                        ", lenY = " + Convert.ToDouble(rasterHeightTB.Text) +
                        ", stepY = " + Convert.ToDouble(rasterStepSizeTB.Text) +
                        ", speed = " + Convert.ToDouble(rasterSpeedTB.Text)
                        );
                    var resp = waitForResponse("raster resp", 2000);
                    if (resp == "")
                        MessageBox.Show("The raster pattern movement could not be started");
                    else
                    {
                        if (getArgs(resp)["answer"] == "no")
                            MessageBox.Show(getArgs(resp)["message"]);
                        else
                        {
                            ((Button)sender).Text = "Stop";
                        }
                    }
                }
                catch { }
            }
            else
            {
                SendCom("raster stop");
                var resp = waitForResponse("raster stop resp", 2000);
                if (resp != "")
                {
                    if (getArgs(resp)["answer"] == "stopped")
                        ((Button)sender).Text = "Begin";
                }
            }
            inTick = false;
            inRasterStart = false;
        }

        private void abortB_Click(object sender, EventArgs e)
        {
            SendCom("abort");
            SendCom("abort");
        }
    }
}
