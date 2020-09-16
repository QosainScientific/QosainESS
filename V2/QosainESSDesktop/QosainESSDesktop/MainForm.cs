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
using System.Threading;
using System.IO;

namespace QosainESSDesktop
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            if (!File.Exists("textBoxTexts.txt") && File.Exists("textBoxTexts_defaults.txt"))
                File.Copy("textBoxTexts_defaults.txt", "textBoxTexts.txt");
            InitializeComponent(); 
            this.panel4.BackgroundImage = QosainESSDesktop.Properties.Resources.QosainLogoTransparent;
            this.windowedModeB.BackgroundImage = QosainESSDesktop.Properties.Resources.NormalDim;
            this.windowedModeB.HoverBackgroundImage = global::QosainESSDesktop.Properties.Resources.NormalHighlight;
            this.closeB.BackgroundImage = global::QosainESSDesktop.Properties.Resources.CloseDim;
            this.closeB.HoverBackgroundImage = global::QosainESSDesktop.Properties.Resources.CloseHighlighted;
            this.minimizeB.BackgroundImage = global::QosainESSDesktop.Properties.Resources.MinimizeDim;
            this.minimizeB.HoverBackgroundImage = global::QosainESSDesktop.Properties.Resources.MinimizeDim;
        }          

        private void Form1_Load(object sender, EventArgs e)
        {
            flowrateUS.Initialize(syringeFlowRateTB, new Units.mlPerMinute(), new Units.mlPerSecond(), new Units.ccPerMinutes(), new Units.ulPerMinutes(), new Units.ulPerSecond());
            syringeDiaUnitChanger.Initialize(syringeDiaTB, new Units.mm(), new Units.cm(), new Units.um(), new Units.Inch());
            widthus.Initialize(rasterWidthTB, new Units.mm(), new Units.cm(), new Units.um(), new Units.Inch());
            heightus.Initialize(rasterHeightTB, new Units.mm(), new Units.cm(), new Units.um(), new Units.Inch());
            stepSizeUs.Initialize(rasterStepSizeTB, new Units.mm(), new Units.cm(), new Units.um(), new Units.Inch());
            speedus.Initialize(rasterSpeedTB, new Units.mmPerSecond(), new Units.mmPerMinute(), new Units.inchesPerSecond(), new Units.inchesPerMinute());
            timeus.Initialize(syringeTimeLimitTB, new Units.seconds(), new Units.minutes(), new Units.hours());
            volumeus.Initialize(syringeVolumeLimitTB, new Units.ml(), new Units.ul(), new Units.cc());
            rasterParamTB_TextChanged(null, null);
            var device = SerialInstrumentFinder.GetDevice("Qosain ESS");
            if (device != null)
            {
                Channel = device.SerialPort;
                if (device.SerialPort == null)
                    device = null;
            }
            //if (device == null)
            //{
            //    MessageBox.Show("The application cannot run without a device and will now exit. (E01)");
            //    Environment.Exit(1);
            //}
            //else
            {
                new Thread(() => { Thread.Sleep(300); Invoke(new MethodInvoker(() => { dataPort_Connected(); })); }).Start();
            }
        }
        
        private void closeB_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string ProcessString { get { return rasterEnabledCB.Checked ? "Process" : "Pumping"; } }
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
            if (Channel == null)
            {
                MessageBox.Show("Kindly connect to the hardware first (E02)");
                return;
            }
            Channel.Write(new UTF8Encoding().GetBytes(com + "\n"), 0, com.Length + 1);
            //sendInParts(com + "\n");
        }
        void sendInParts(string com)
        {
            while (com.Length > 0)
            {
                int perPacket = 16;
                if (com.Length < perPacket)
                    perPacket = com.Length;
                Channel.Write(new UTF8Encoding().GetBytes(com.Substring(0, perPacket)), 0, perPacket);
                com = com.Substring(perPacket);
                System.Threading.Thread.Sleep(100);
            }
        }
        string waitForResponse(string com, int timeOut)
        {
            while (inTick && timeOut > 0)
            {
                System.Threading.Thread.Sleep(100);
                timeOut -= 100;
            }
            if (timeOut <= 0)
                return "";
            inTick = true;

            var start = DateTime.Now;
            var rec = "";
            while (getCommand(rec) != com && timeOut > 0)
            {
                rec = ReceiveCom(timeOut);
                timeOut -= (int)(DateTime.Now - start).TotalMilliseconds;
                start = DateTime.Now;
            }
            if (getCommand(rec) == com)
            {
                inTick = false;
                return rec;
            }
            else
            {
                inTick = false;
                return "";
            }
        }
        string ReceiveCom(int timeOut = 1000)
        {
            if (Channel == null)
                return "";
            if (!Channel.IsOpen)
            {
                MessageBox.Show("The connection to the device was broken abnormally. The system must exit now. (E03)");
                Environment.Exit(1);
                return "";
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < timeOut; i+=30)
            {
                if (Channel == null)
                    return "";

                if (!Channel.IsOpen)
                {
                    MessageBox.Show("The connection to the device was broken abnormally. The system must exit now. (E04)");
                    Environment.Exit(1);
                    return "";
                }
                while (Channel.BytesToRead > 0)
                {
                    char c = (char)Channel.ReadByte();
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
        System.Windows.Forms.Timer poller;
        private void dataPort_Connected()
        {
            //var resp = waitForResponse("begin", 5000);
            //if (resp != "begin")
            //{
            //    MessageBox.Show("Could not connect to the hardware");
            //    Environment.Exit(1);
            //    return;
            //}
            rasterView1.UpdateViewXY(0, 0);
            new Thread(() =>
            {
                while (firstHomingStatus == -1) Thread.Sleep(30); 
                if (firstHomingStatus == 0) this.Invoke(new MethodInvoker(() =>
                {
                    SendCom("home xy");
                }));
            }).Start() ;

                poller = new System.Windows.Forms.Timer();
            poller.Interval = 30;
            poller.Tick += Poller_Tick;
            poller.Enabled = true;
            //dataPort.Visible = false;
            coatB.Visible = true;
            abortB.Visible = true;
        }

        bool inTick = false;
        int firstHomingStatus = -1; // -1 is undefined, 0 is no, 1 is done
        SerialPort Channel;
        private void Poller_Tick(object sender, EventArgs e)
        {
            if (inTick)
                return;
            inTick = true;
            //SendCom("status");
            if (Channel == null)
                return;
            if (!Channel.IsOpen)
                return;
            while (Channel.BytesToRead > 0)
            {
                var raw = ReceiveCom(500);
                var name = getCommand(raw);
                var args = getArgs(raw);
                try
                {
                    if (name == "status")
                    {
                        xCoordL.Text = args["x"];
                        yCoordL.Text = args["y"];
                        if (args["xy stage"] != "{last}")
                            xyStageStatusL.Text = args["xy stage"];
                        if (firstHomingStatus == -1)
                        {
                            if (args["xy stage"] != "{last}")
                            {
                                if (args["xy stage"] != "Idle")
                                    firstHomingStatus = 1;
                                else
                                {
                                    if (Math.Abs(double.Parse(args["x"])) > 0.5 || Math.Abs(double.Parse(args["y"])) > 0.5)
                                        firstHomingStatus = 1;
                                    else
                                        firstHomingStatus = 0;
                                }
                            }
                        }
                        if (args["pump"] != "{last}")
                            pumpStatusL.Text = args["pump"];
                        if (Convert.ToDouble(args["progress"]) >= 0)
                            plainProgressBar1.Visible = true;
                        else
                            plainProgressBar1.Visible = false;
                        if (Convert.ToDouble(args["progress"]) >= 0)
                            plainProgressBar1.Value = Convert.ToDouble(args["progress"]);
                        if (xyStageStatusL.Text == "Moving" || xyStageStatusL.Text == "Coating")
                        {
                            xyStageStatusL.BackColor = Color.FromArgb(255, 128, 128);
                            rasterView1.UpdateViewXY(Convert.ToSingle(args["x"]), Convert.ToSingle(args["y"]));
                        }
                        else if (xyStageStatusL.Text.Contains("Homing"))
                        {
                            xyStageStatusL.BackColor = Color.FromArgb(128, 255, 128);
                            if (args["pump"] != "{last}")
                                xCoordL.Text = "Homing";
                            yCoordL.Text = "Homing";
                        }
                        else if (xyStageStatusL.Text == "Idle")
                            xyStageStatusL.BackColor = Color.DimGray;

                        if (pumpStatusL.Text == "Pumping")
                            pumpStatusL.BackColor = Color.FromArgb(255, 128, 128);
                        else if (pumpStatusL.Text.Contains("Homing"))
                            pumpStatusL.BackColor = Color.FromArgb(128, 255, 128);
                        else if (pumpStatusL.Text == "Idle")
                            pumpStatusL.BackColor = Color.DimGray;
                        //Application.DoEvents();
                    }
                    else if (name == "info")
                    {
                        if (args["tag"] == "homing")
                            ;
                        else if (args["tag"] == "..")
                        { }
                        else
                        {
                            var msg = args["message"];
                        }
                    }
                    else if (name == "coat end")
                    {
                        rasterEnded();
                        coatB.Text = "Begin " + ProcessString;

                        MessageBox.Show(ProcessString + " finished successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (name != "")
                    {
                        args = args;
                    }
                }
                catch { }
            }
            inTick = false;
        }

        private void dataPort_Disconnected(object sender, EventArgs e)
        {
            if (poller != null)
                poller.Enabled = false;
            poller = null;
        }
             

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            new System.Threading.Thread(() =>
            {
                Channel?.Close();
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(1);
            }).Start();
        }                      
        private void dataPort_DevicesRefreshed(object sender, EventArgs e)
        {                                                 
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
        private void label2_Click(object sender, EventArgs e)
        {

        }

        bool inPumpStart = false;

        void pumpStarted()
        { 
            //pumpBeginB.Text = "Stop";
            syringeDiaTB.Enabled = false;
            syringeFlowRateTB.Enabled = false;
            syringeTimeLimitTB.Enabled = false;
            syringeVolumeLimitTB.Enabled = false;

            rasterEnabledCB.Enabled = false;
            enableTimeLimit.Enabled = false;
            enableVolumeLimitB.Enabled = false;
            spManualP.Enabled = false;
        }
        void pumpEnded()
        {
            //pumpBeginB.Text = "Begin";
            syringeDiaTB.Enabled = true;
            syringeFlowRateTB.Enabled = true;
            syringeTimeLimitTB.Enabled = true;
            syringeVolumeLimitTB.Enabled = true;

            rasterEnabledCB.Enabled = true;
            enableTimeLimit.Enabled = true;
            enableVolumeLimitB.Enabled = true;

            spManualP.Enabled = true;
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
        void rasterStarted()
        {                               
            plainProgressBar1.Visible = true;
            // ??????????? progress missing            
            plainProgressBar1.Started();

            rasterWidthTB.Enabled = false;
            rasterHeightTB.Enabled = false;
            rasterStepSizeTB.Enabled = false;
            rasterSpeedTB.Enabled = false;
            rasterPad.Enabled = false;
            rasterCoatsTB.Enabled = false;
        }
        void rasterEnded()
        {                                 
            rasterView1.endRaster();
            plainProgressBar1.Ended();

            rasterWidthTB.Enabled = true;
            rasterHeightTB.Enabled = true;
            rasterStepSizeTB.Enabled = true;
            rasterSpeedTB.Enabled = true;
            rasterParamTB_TextChanged(null, null);
            rasterPad.Enabled = true;
            rasterCoatsTB.Enabled = true;
        }
        bool beginCoating()
        {
            try
            {
                if (rasterEnabledCB.Checked)
                {
                    if (rasterWidthTB.Value.StandardValue <= 0)
                        throw new Exception("Raster area width can only be a valid positive number.");
                    if (rasterHeightTB.Value.StandardValue <= 0)
                        throw new Exception("Raster area height can only be a valid positive number.");
                    if (rasterStepSizeTB.Value.StandardValue <= 0)
                        throw new Exception("Raster step size can only be a valid positive number.");
                    if (syringeFlowRateTB.Value.StandardValue <= 0)
                        throw new Exception("Syringe flow rate can only be a valid positive number.");
                    if (rasterSpeedTB.Value.StandardValue <= 0)
                        throw new Exception("Travel speed can only be a valid positive number.");
                }
                double mxd = Convert.ToDouble(syringeVolumeLimitTB.Value.As(new Units.ml())) * 1000 / (Math.Pow(Convert.ToSingle(syringeDiaTB.Value.As(new Units.mm())) / 2, 2) * (float)Math.PI);
                double q = Convert.ToSingle(syringeFlowRateTB.Value.As(new Units.mlPerMinute())) * 1000 / 60.0F / (Math.Pow(Convert.ToSingle(syringeDiaTB.Value.As(new Units.ml())) / 2, 2) * (float)Math.PI);
                SendCom("set coat:" +
                    "lenX=" + Convert.ToSingle(rasterWidthTB.Value.As(new Units.ml())) +
                    ",lenY=" + Convert.ToSingle(rasterHeightTB.Value.As(new Units.ml())) +
                    ",stepY=" + Convert.ToSingle(rasterStepSizeTB.Value.As(new Units.ml())) +
                    ",coats=" + Convert.ToSingle(rasterCoatsTB.Text) +
                    ",speed=" + Convert.ToSingle(rasterSpeedTB.Value.As(new Units.mmPerSecond())) +
                    ",Q=" + q +
                    ",mxt=" + (enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Value.As(new Units.seconds())) * 60 : -1) +
                    ",mxd=" + (enableVolumeLimitB.Checked ? mxd : -1) + 
                    ",rstr=" + (rasterEnabledCB.Checked ? "1" : "0")
                    ); 
                var resp = waitForResponse("coat resp", 2000);
                if (resp == "")
                    MessageBox.Show(ProcessString + " could not be started. (E07)");
                else
                {
                    if (getArgs(resp)["answer"] == "no")
                        MessageBox.Show(getArgs(resp)["message"] + " (E08)");
                    else
                    {
                        rasterView1.beginRaster();
                        rasterStarted();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {                        
                MessageBox.Show(ex.Message);
            }
            return false;
        }
        void endRaster()
        {
            SendCom("abort");
            var resp = waitForResponse("abort resp", 2000);
            if (resp != "")
            {
                if (getArgs(resp)["answer"] == "stopped")
                {
                    rasterEnded();
                }
            }
        }            

        private void abortB_Click(object sender, EventArgs e)
        {
            endRaster();
            coatB.Text = "Begin " + ProcessString;
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rasterParamTB_TextChanged(object sender, EventArgs e)
        {

            try
            {
                rasterView1.UpdateViewRaster(Convert.ToSingle(rasterWidthTB.Value.As(new Units.mm())), Convert.ToSingle(rasterHeightTB.Value.As(new Units.mm())), Convert.ToSingle(rasterStepSizeTB.Value.As(new Units.mm())));

                float dist = 0;
                float rWidth = Convert.ToSingle(rasterWidthTB.Value.As(new Units.mm()));
                float rHeight = Convert.ToSingle(rasterHeightTB.Value.As(new Units.mm()));
                float step = Convert.ToSingle(rasterStepSizeTB.Value.As(new Units.mm()));
                float speed = Convert.ToSingle(rasterSpeedTB.Value.As(new Units.mmPerSecond()));
                if (step == 0)
                    throw new Exception();
                for (float y = 0; y < rHeight; y += step * 2)
                {
                    dist += rWidth;
                    if (y + step <= rHeight)
                        dist += step + rWidth;
                    if (y + 2 * step <= rHeight)
                        dist += step;
                }
                dist *= Convert.ToSingle(rasterCoatsTB.Text);
            }
            catch { rasterView1.HideRaster();  }
        }

        private void coatB_Click(object sender, EventArgs e)
        {                
            if (coatB.Text == "Pause")
            {                   
                SendCom("pause coat");
                coatB.Text = "Resume";

            }
            else if (coatB.Text == "Resume")
            {
                SendCom("resume coat");
                coatB.Text = "Pause";

            }
            else      // begin
            {                    
                if (beginCoating())   
                    coatB.Text = "Pause";
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rasterEnabledCB_CheckedChanged(object sender, EventArgs e)
        {
            new Thread(() => { Thread.Sleep(30); Invoke(new MethodInvoker(() => {
                rasterEnabledCB.Text = rasterEnabledCB.Checked ? "Enabled" : "Disabled";
                if (coatB.Text.StartsWith("Begin"))
                    coatB.Text = "Begin " + ProcessString;
            })); }).Start();
        }
    }
}
