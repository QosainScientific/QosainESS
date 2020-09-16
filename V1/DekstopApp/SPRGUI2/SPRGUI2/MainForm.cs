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
            rasterParamTB_TextChanged(null, null);
            spAutoB_Click(null, null);
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
            //sendInParts(com + "\n");
        }
        void sendInParts(string com)
        {
            while (com.Length > 0)
            {
                int perPacket = 16;
                if (com.Length < perPacket)
                    perPacket = com.Length;
                dataPort.Channel.Write(new UTF8Encoding().GetBytes(com.Substring(0, perPacket)), 0, perPacket);
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
            rasterView1.UpdateViewXY(0, 0);
            SendCom("home all");
            poller = new Timer();
            poller.Interval = 30;
            poller.Tick += Poller_Tick;
            poller.Enabled = true;
            dataPort.Visible = false;
            coatB.Visible = true;
            abortB.Visible = true;
        }

        bool inTick = false;
        private void Poller_Tick(object sender, EventArgs e)
        {
            if (inTick)
                return;
            inTick = true;
            //SendCom("status");
            if (dataPort.Channel.BytesToRead > 0)
            {
                var raw = ReceiveCom(500);
                var name = getCommand(raw);
                var args = getArgs(raw);
                if (name == "status")
                {
                    xCoordL.Text = args["x"];
                    yCoordL.Text = args["y"];
                    xyStageStatusL.Text = args["xy stage"];
                    pumpStatusL.Text = args["pump"];
                    plainProgressBar1.Visible = args["progress"] != "-1";
                    plainProgressBar1.Value = Convert.ToDouble(args["progress"]);
                    if (xyStageStatusL.Text == "Moving" || xyStageStatusL.Text == "Coating")
                    {
                        xyStageStatusL.BackColor = Color.FromArgb(255, 128, 128);
                        rasterView1.UpdateViewXY(Convert.ToSingle(args["x"]), Convert.ToSingle(args["y"]));
                    }
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
                    coatB.Text = "Begin Coating";

                    MessageBox.Show("Coating finished successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (name != "")
                {
                    args = args;
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
            new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
            })).Start();
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

        private void RasterParamTB_TextChanged(object sender, System.EventArgs e)
        {
        }
        private void spAutoB_Click(object sender, EventArgs e)
        {
            int inc = 220;
            for (int i = spModesSC.SplitterDistance; i < spModesSC.Height - spManualB.Top - spManualB.Height; i+=inc)
            {
                System.Threading.Thread.Sleep(20);
                spModesSC.SplitterDistance = i;
                Application.DoEvents();
                inc = (int)Math.Round(inc * 0.79F) + 1;
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
            int inc = 250;
            for (int i = spModesSC.SplitterDistance; i >= spAutoB.Top + spAutoB.Height; i -= inc)
            {

                System.Threading.Thread.Sleep(20);
                spModesSC.SplitterDistance = i;
                Application.DoEvents();
                inc = (int)Math.Round(inc * 0.78F) + 1;
            }
            spModesSC.SplitterDistance = spAutoB.Top + spAutoB.Height;
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

            enableTimeLimit.Enabled = false;
            enableVolumeLimitB.Enabled = false;
            spManualB.Enabled = false;
        }
        void pumpEnded()
        {
            //pumpBeginB.Text = "Begin";
            syringeDiaTB.Enabled = true;
            syringeFlowRateTB.Enabled = true;
            syringeTimeLimitTB.Enabled = true;
            syringeVolumeLimitTB.Enabled = true;

            enableTimeLimit.Enabled = true;
            enableVolumeLimitB.Enabled = true;

            spManualB.Enabled = true;
        }
        bool beginPumping()
        {
            try
            {

                SendCom("pst:" +
                    "r=" + Convert.ToSingle(syringeFlowRateTB.Text) * 1000 * 60 / 3600.0F / (Math.Pow(Convert.ToSingle(syringeDiaTB.Text) / 2, 2) * (float)Math.PI) +
                    ",mxt=" + (enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Text) * 60000 : -1) +
                    ",mxd=" + (enableVolumeLimitB.Checked ? Convert.ToDouble(syringeVolumeLimitTB.Text) * 1000 / (Math.Pow(Convert.ToSingle(syringeDiaTB.Text) / 2, 2) * (float)Math.PI) : -1)
                    );
                var resp = waitForResponse("pump resp", 2000);
                if (resp == "")
                    MessageBox.Show("The pump could not be started");
                else
                {
                    if (getArgs(resp)["answer"] == "yes")
                    {
                        pumpStarted();
                        return true;
                    }
                    else
                        MessageBox.Show(getArgs(resp)["message"]);
                }
            }
            catch
            { }
            return false;
        }
        void endPumping()
        {
            SendCom("pump stop");
            var resp = waitForResponse("pump stop resp", 2000);
            if (resp != "")
            {
                if (getArgs(resp)["answer"] == "stopped")
                    pumpEnded();
            }
        }
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
                beginPumping();
            }
            else
            {
                endPumping();
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
                if (Convert.ToDouble(rasterWidthTB.Text) <= 0)
                    throw new Exception("Coating area width can only be a valid positive number.");
                if (Convert.ToDouble(rasterHeightTB.Text) <= 0)
                    throw new Exception("Coating area height can only be a valid positive number.");
                if (Convert.ToDouble(rasterStepSizeTB.Text) <= 0)
                    throw new Exception("Raster step size can only be a valid positive number.");
                if (Convert.ToDouble(syringeFlowRateTB.Text) <= 0)
                    throw new Exception("Syringe flow rate can only be a valid positive number.");
                if (Convert.ToDouble(rasterSpeedTB.Text) <= 0 || Convert.ToDouble(rasterSpeedTB.Text) > 5)
                    throw new Exception("The travel speed you entered is not within the possible hardware range, (0, 5] mm/s");
                double mxd = Convert.ToDouble(syringeVolumeLimitTB.Text) * 1000 / (Math.Pow(Convert.ToSingle(syringeDiaTB.Text) / 2, 2) * (float)Math.PI);
                double q = Convert.ToSingle(syringeFlowRateTB.Text) * 1000 / 60.0F / (Math.Pow(Convert.ToSingle(syringeDiaTB.Text) / 2, 2) * (float)Math.PI);
                SendCom("set coat:" +
                    "lenX=" + Convert.ToSingle(rasterWidthTB.Text) +
                    ",lenY=" + Convert.ToSingle(rasterHeightTB.Text) +
                    ",stepY=" + Convert.ToSingle(rasterStepSizeTB.Text) +
                    ",coats=" + Convert.ToSingle(rasterCoatsTB.Text) +
                    ",speed=" + Convert.ToSingle(rasterSpeedTB.Text) +
                    ",Q=" + q +
                    ",mxt=" + (enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Text) * 60 : -1) +                    
                    ",mxd=" + (enableVolumeLimitB.Checked ? mxd : -1)
                    ); 
                var resp = waitForResponse("coat resp", 2000);
                if (resp == "")
                    MessageBox.Show("Coating process could not be started");
                else
                {
                    if (getArgs(resp)["answer"] == "no")
                        MessageBox.Show(getArgs(resp)["message"]);
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
            coatB.Text = "Begin Coating";
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rasterParamTB_TextChanged(object sender, EventArgs e)
        {

            try
            {
                rasterView1.UpdateViewRaster(Convert.ToSingle(rasterWidthTB.Text), Convert.ToSingle(rasterHeightTB.Text), Convert.ToSingle(rasterStepSizeTB.Text));

                float dist = 0;
                float rWidth = Convert.ToSingle(rasterWidthTB.Text);
                float rHeight = Convert.ToSingle(rasterHeightTB.Text);
                float step = Convert.ToSingle(rasterStepSizeTB.Text);
                float speed = Convert.ToSingle(rasterSpeedTB.Text);
                for (float y = 0; y < rHeight; y += step * 2)
                {
                    dist += rWidth;
                    if (y + step <= rHeight)
                        dist += step + rWidth;
                    if (y + 2 * step <= rHeight)
                        dist += step;
                }
                dist *= Convert.ToSingle(rasterCoatsTB.Text);
                // ?????????? progress missing

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
    }
}
