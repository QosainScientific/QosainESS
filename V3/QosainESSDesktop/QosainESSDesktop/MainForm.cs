using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using FivePointNine.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using SharpDX.Direct3D9;
using RJCP.IO.Ports;

namespace QosainESSDesktop
{
    public partial class MainForm : Form
    {
        public ESSMachine Machine;
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
            setTempUS.Initialize(setTempTB, new Units.kelvin(), new Units.celsius(), new Units.fahrenheit());
            cylinderSpeedTB.NotifyControlCreated();
            cylinderSpeedUS.Initialize(cylinderSpeedTB, new Units.radpersec(), new Units.revpermin(), new Units.revpersec());

            try
            {
                ((CheckBox)this.Controls.Find(File.ReadAllText("programMode.txt"), true)[0]).Checked = true;
            }
            catch { }
            rasterParamTB_TextChanged(null, null);
            ConnectedDevice = SerialInstrumentFinder.GetDevice("Qosain ESS");
            if (ConnectedDevice == null) // user closed our machine selector
            {
                // Comment this to allow using the SW without the hardware
                // Close();
            }
            else
            {
                if (ConnectedDevice.HasMarlin)
                    Machine = new MarlinESSMachine(ConnectedDevice.SerialPort);
                else
                    Machine = new ArduinoESSMachine(Channel);
                Machine.OnStatusArgsUpdate += applyStatusArgs;
                ESSMachine.OnMachineError += ESSMachine_OnMachineError;
                if (ConnectedDevice != null)
                {
                    Channel = ConnectedDevice.SerialPort;
                    if (ConnectedDevice.SerialPort == null)
                        ConnectedDevice = null;
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
        }

        private void ESSMachine_OnMachineError(string message)
        {
            MessageBox.Show(message);
            Close();
        }

        private void closeB_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string ProcessString { get { return rasterEnabledCB.Checked ? "Process" : (cylinderEnabledCB.Checked ? "Spinning" : "Pumping"); } }
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
                if (firstHomingStatus == 0)
                    this.Invoke(new MethodInvoker(() =>
                    {
                        Machine.SendCom("home xy");
                    }));
            }).Start();

            poller = new System.Windows.Forms.Timer();
            poller.Interval = 30;
            poller.Tick += Machine.Poller_Tick;

            poller.Enabled = true;
            //dataPort.Visible = false;
            coatB.Visible = true;
            abortB.Visible = true;
        }

        int firstHomingStatus = -1; // -1 is undefined, 0 is no, 1 is done
        SerialPortStream Channel;
        float[] backupXYZ = new float[3];
        void applyStatusArgs(string name, Dictionary<string, string> args)
        {
            try
            {
                Invoke(new MethodInvoker(() =>
                {
                    try
                    {
                        if (name == "status")
                        {
                            try
                            {
                                backupXYZ[0] = float.Parse(args["x"]);
                                backupXYZ[1] = float.Parse(args["y"]);
                                backupXYZ[2] = float.Parse(args["z"]);
                            }
                            catch { }
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
                            if (args.ContainsKey("cylinder"))
                            {
                                cylinderStatusL.Text = args["cylinder"];
                            }
                            if (args.ContainsKey("temp"))
                            {
                                try
                                {
                                    double v = double.Parse(args["temp"]);
                                    var tq = new Quantity(v, new Units.celsius(), false);
                                    actualTempL.Text = tq.As(setTempTB.Value.CurrentUnit);
                                }
                                catch { actualTempL.Text = ">" + args["temp"]; }
                            }
                            if (args["pump"] != "{last}")
                                pumpStatusL.Text = args["pump"];
                            if (Convert.ToDouble(args["progress"]) >= 0)
                            {
                                plainProgressBar1.Visible = !coatB.Text.StartsWith("Begin");
                                //if (!rasterEnabledCB.Checked)
                                //{
                                //if (!enableVolumeLimitB.Checked && !enableTimeLimit.Checked)
                                //    plainProgressBar1.Visible = false;
                                //else
                                plainProgressBar1.Visible = true;
                                //}
                                //else
                                //    plainProgressBar1.Visible = true;
                            }
                            else
                                plainProgressBar1.Visible = false;
                            if (coatB.Text.ToLower().StartsWith("begin"))
                            {
                                plainProgressBar1.Visible = false;
                                materialPumpedL.Visible = false;
                            }
                            if (Convert.ToDouble(args["progress"]) >= 0)
                            {
                                SetProgressBar(double.Parse(args["progress"]));
                            }
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

                            if (cylinderStatusL.Text == "Spinning")
                                cylinderStatusL.BackColor = Color.FromArgb(255, 128, 128);
                            else
                                cylinderStatusL.BackColor = Color.DimGray;

                            if (pumpStatusL.Text == "Pumping")
                                pumpStatusL.BackColor = Color.FromArgb(255, 128, 128);
                            else
                                pumpStatusL.BackColor = Color.DimGray;
                            Application.DoEvents();
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
                            coatB.Visible = true;
                            coatB.Text = "Begin " + ProcessString;
                            MessageBox.Show(ProcessString + " finished successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Machine.InPlaceReset(backupXYZ);
                        }
                        else if (name != "")
                        {
                        }
                    }
                    catch { }
                }));
            }
            catch { }
        }

        private void SetProgressBar(double progress)
        {
            plainProgressBar1.Value = progress;
            // Also update on the material pumped
            var dt = (DateTime.Now - RasterStartedAt).TotalSeconds - timeInPause;
            var dQ = dt * setPumpQ; // ml
            materialPumpedL.Text =
                "Material pumped: " +
                dQ.ToString("F2") + "ml";
            //if (rasterEnabledCB.Checked)
            //{
            //    var coats = Convert.ToSingle(rasterCoatsTB.Text);
            //    var S = rasterView1.GetTravelDistance() * coats;
            //    var v = double.Parse(rasterSpeedTB.Value.As(new Units.mmPerSecond()));
            //    var t_actual = S / v;
            //    double qUlPs = Convert.ToSingle(syringeFlowRateTB.Value.As(new Units.mlPerMinute())) * 1000 / 60.0F;
            //    var f_actual = qUlPs * t_actual;

            //    double f_volumeLimit = enableVolumeLimitB.Checked ? Convert.ToDouble(syringeVolumeLimitTB.Value.As(new Units.ul())) : f_actual;
            //    double t_timeLimit = enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Value.As(new Units.seconds())) : t_actual;

            //    double f_timeLimit = qUlPs * t_timeLimit;
            //    double t_volumeLimit = f_volumeLimit / qUlPs;

            //    if (t_actual <= t_volumeLimit && t_actual <= t_timeLimit) // not limited by any
            //        plainProgressBar1.Value = progress;
            //    // else, let time decide
            //}
            //else
            //{
            //    var t_actual = double.MaxValue;
            //    double qUlPs = Convert.ToSingle(syringeFlowRateTB.Value.As(new Units.mlPerMinute())) * 1000 / 60.0F;
            //    var f_actual = qUlPs * t_actual;

            //    double f_volumeLimit = enableVolumeLimitB.Checked ? Convert.ToDouble(syringeVolumeLimitTB.Value.As(new Units.ul())) : f_actual;
            //    double t_timeLimit = enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Value.As(new Units.seconds())) : t_actual;

            //    double f_timeLimit = qUlPs * t_timeLimit;
            //    double t_volumeLimit = f_volumeLimit / qUlPs;

            //    if (t_actual <= t_volumeLimit && t_actual <= t_timeLimit) // not limited by any
            //        ;
            //    else if (t_volumeLimit <= t_timeLimit) // volume limit
            //        plainProgressBar1.Value = progress;
            //}
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
            string[] comTable = { "y+", "y++", "y-", "y--", "x-", "x--", "x+", "x++" };
            if (id >= 0 && id < comTable.Length)
                Machine.SendCom(comTable[id]);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void spUpB_Click(object sender, EventArgs e)
        {
            Machine.SendCom("z+");
        }

        private void spUpUpB_Click(object sender, EventArgs e)
        {
            Machine.SendCom("z++");
        }

        private void spDownB_Click(object sender, EventArgs e)
        {
            Machine.SendCom("z-");
        }

        private void spDownDownB_Click(object sender, EventArgs e)
        {
            Machine.SendCom("z--");
        }
        private void spTopB_Click(object sender, EventArgs e)
        {
            Machine.SendCom("ztop");
        }

        bool inRasterStart = false;
        DateTime RasterStartedAt;
        float setPumpQ = 0; // ml/s
        double timeInPause = 0;
        DateTime pausedAt;
        private SerialInstrumentFinder.SerialDevice ConnectedDevice;

        void processStarted()
        {                               
            plainProgressBar1.Visible = true;
            // ??????????? progress missing            
            plainProgressBar1.Started();
            estimateMaterialB.Visible = false;
            materialPumpedL.Visible = true;

            panel1.Enabled = false;
            RasterStartedAt = DateTime.Now;
            setPumpQ = float.Parse(syringeFlowRateTB.Value.As(new Units.mlPerSecond()));

        }
        void rasterEnded()
        {
            rasterView1.endRaster();
            plainProgressBar1.Ended();
            estimateMaterialB.Visible = true;
            materialPumpedL.Visible = false;

            panel1.Enabled = true;
            rasterParamTB_TextChanged(null, null);
        }
        bool beginCoating(int retries)
        {
            //try
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
                if (cylinderEnabledCB.Checked)
                {
                    var valRPM = float.Parse(cylinderSpeedTB.Value.As(new Units.revpermin()));
                    float minRPM = 0.01F;
                    float maxRPM = 100;
                    if (valRPM < minRPM || valRPM > maxRPM)
                        throw new Exception("Please enter a target cylinder speed within the range [" +
                            new Quantity(minRPM, new Units.revpermin(), false).As(cylinderSpeedTB.Value.CurrentUnit) +
                            cylinderSpeedTB.Value.CurrentUnit.Suffix + ", " +
                            new Quantity(maxRPM, new Units.revpermin(), false).As(cylinderSpeedTB.Value.CurrentUnit) +
                            cylinderSpeedTB.Value.CurrentUnit.Suffix + "]");
                }
                double mxd = Convert.ToDouble(syringeVolumeLimitTB.Value.As(new Units.ml())) * 1000 / (Math.Pow(Convert.ToSingle(syringeDiaTB.Value.As(new Units.mm())) / 2, 2) * (float)Math.PI);
                double q = Convert.ToSingle(syringeFlowRateTB.Value.As(new Units.mlPerMinute())) * 1000 / 60.0F / (Math.Pow(Convert.ToSingle(syringeDiaTB.Value.As(new Units.mm())) / 2, 2) * (float)Math.PI);

                
                Machine.SendCom("set coat:" +
                    "lenX=" + Convert.ToSingle(rasterWidthTB.Value.As(new Units.mm())) +
                    ",lenY=" + Convert.ToSingle(rasterHeightTB.Value.As(new Units.mm())) +
                    ",stepY=" + Convert.ToSingle(rasterStepSizeTB.Value.As(new Units.mm())) +
                    ",coats=" + Convert.ToSingle(rasterCoatsTB.Text) +
                    ",speed=" + Convert.ToSingle(rasterSpeedTB.Value.As(new Units.mmPerSecond())) +
                    ",cylrps=" + Convert.ToSingle(cylinderSpeedTB.Value.As(new Units.revpersec())) +
                    ",Q=" + q +
                    ",mxt=" + (enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Value.As(new Units.minutes())) * 60 : -1) +
                    ",mxd=" + (enableVolumeLimitB.Checked ? mxd : -1) +
                    ",rstr=" + (rasterEnabledCB.Checked ? "1" : "0") +
                    ",cyl=" + (cylinderEnabledCB.Checked ? "1" : "0")
                    );
                if (enableTimeLimit.Checked)
                    plainProgressBar1.ForceTimeEstimate(Convert.ToDouble(syringeTimeLimitTB.Value.As(new Units.minutes())) * 60);
                if (enableVolumeLimitB.Checked && rasterEnabledCB.Checked) // in case raster is enabled, valume limit can be estimated with time
                {
                    // check if volume limit is active

                    var coats = Convert.ToSingle(rasterCoatsTB.Text);
                    var S = rasterView1.GetTravelDistance() * coats;
                    var v = double.Parse(rasterSpeedTB.Value.As(new Units.mmPerSecond()));
                    var t_actual = S / v;
                    double qUlPs = Convert.ToSingle(syringeFlowRateTB.Value.As(new Units.mlPerMinute())) * 1000 / 60.0F;
                    var f_actual = qUlPs * t_actual;

                    double f_volumeLimit = enableVolumeLimitB.Checked ? Convert.ToDouble(syringeVolumeLimitTB.Value.As(new Units.ul())) : f_actual;
                    double t_timeLimit = enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Value.As(new Units.seconds())) : t_actual;

                    double f_timeLimit = qUlPs * t_timeLimit;
                    double t_volumeLimit = f_volumeLimit / qUlPs;

                    if (t_actual <= t_volumeLimit && t_actual <= t_timeLimit) // not limited by any
                        ;
                    else if (t_actual > t_volumeLimit && t_volumeLimit <= t_timeLimit) // volume limit
                        plainProgressBar1.ForceTimeEstimate(t_volumeLimit);
                }
                var resp = Machine.waitForResponse("coat resp", 2000);
                if (resp == "")
                {
                    Machine.InPlaceReset(backupXYZ);
                    if (retries > 0)
                        return beginCoating(retries - 1);
                    MessageBox.Show(ProcessString + " could not be started, kindly try again. (E07)");
                    return false;
                }

                else
                {
                    if (ESSMachine.getArgs(resp)["answer"] == "no")
                        MessageBox.Show(ESSMachine.getArgs(resp)["message"] + " (E08)");
                    else
                    {
                        rasterView1.beginRaster();
                        coatB.Visible = rasterEnabledCB.Checked;
                        processStarted();
                        return true;
                    }
                }
                timeInPause = 0;
                plainProgressBar1.Reset();
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            return false;
        }
        void endRaster()
        {
            Machine.SendCom("abort");
            var resp = Machine.waitForResponse("abort resp", 2000);
            if (resp != "")
            {
                if (ESSMachine.getArgs(resp)["answer"] == "stopped")
                {
                    rasterEnded();
                }
            }
        }            

        private void abortB_Click(object sender, EventArgs e)
        {
            endRaster();
            coatB.Text = "Begin " + ProcessString;
            coatB.Visible = true;
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
            catch { rasterView1.HideRaster(); }
        }

        private void coatB_Click(object sender, EventArgs e)
        {                
            if (coatB.Text == "Pause")
            {
                pausedAt = DateTime.Now;
                try
                {
                    plainProgressBar1.Pause();
                }
                catch { }
                try
                {
                    Machine.SendCom("pause coat");
                }
                catch { }
                coatB.Text = "Resume";

            }
            else if (coatB.Text == "Resume")
            {
                timeInPause += (DateTime.Now - pausedAt).TotalSeconds;
                Machine.SendCom("resume coat");
                plainProgressBar1.Resume();
                coatB.Text = "Pause";

            }
            else      // begin
            {                    
                if (beginCoating(2))   
                    coatB.Text = "Pause";
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void estimateMaterialB_Click(object sender, EventArgs e)
        {

            if (rasterEnabledCB.Checked)
            {
                StringBuilder sb = new StringBuilder();
                var coats = Convert.ToSingle(rasterCoatsTB.Text);
                var S = rasterView1.GetTravelDistance() * coats;
                var v = double.Parse(rasterSpeedTB.Value.As(new Units.mmPerSecond()));
                var t_actual = S / v;
                double qUlPs = Convert.ToSingle(syringeFlowRateTB.Value.As(new Units.mlPerMinute())) * 1000 / 60.0F;
                var f_actual = qUlPs * t_actual;

                double f_volumeLimit = enableVolumeLimitB.Checked ? Convert.ToDouble(syringeVolumeLimitTB.Value.As(new Units.ul())) : f_actual;
                double t_timeLimit = enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Value.As(new Units.seconds())) : t_actual;

                double f_timeLimit = qUlPs * t_timeLimit;
                double t_volumeLimit = f_volumeLimit / qUlPs;

                if (t_actual <= t_volumeLimit && t_actual <= t_timeLimit) // not limited by any
                {
                    sb.AppendLine("Time: " + new TimeSpan(0, 0, 0, (int)t_actual).ToString(@"hh\:mm\:ss"));
                    sb.AppendLine("Material volume: " + new Quantity(f_actual, new Units.ul(), false).As(new Units.cc(), "F2") + " cc");

                    if (enableVolumeLimitB.Checked && enableTimeLimit.Checked)
                        sb.AppendLine("\r\nThe coating will end BEFORE the set time and material volume limit is reached");
                    else if (enableVolumeLimitB.Checked)
                        sb.AppendLine("\r\nThe coating will end BEFORE the set material volume limit is reached");
                    else if (enableTimeLimit.Checked)
                        sb.AppendLine("\r\nThe coating will end BEFORE the set time limit is reached");
                }
                else if (t_actual > t_volumeLimit && t_volumeLimit <= t_timeLimit) // volume limit
                {
                    sb.AppendLine("Time: " + new TimeSpan(0, 0, 0, (int)t_volumeLimit).ToString(@"hh\:mm\:ss"));
                    sb.AppendLine("Material volume: " + new Quantity(f_volumeLimit, new Units.ul(), false).As(new Units.cc(), "F2") + " cc");

                    sb.AppendLine("\r\nThe coating will stop when the set volume limit is reached");
                }
                else if (t_actual >= t_timeLimit) // time limit
                {
                    sb.AppendLine("Time: " + new TimeSpan(0, 0, 0, (int)t_timeLimit).ToString(@"hh\:mm\:ss"));
                    sb.AppendLine("Material volume: " + new Quantity(f_timeLimit, new Units.ul(), false).As(new Units.cc(), "F2") + " cc");

                    sb.AppendLine("\r\nThe coating will stop when the set time limit is reached");
                }
                if (sb.Length > 0)
                {
                    MessageBox.Show("Please note that these values are just estimates and may vary slightly depending upon the health of the ESS system.\r\n\r\n" + sb.ToString(), "Estimates", MessageBoxButtons.OK);
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                var t_actual = double.MaxValue;
                double qUlPs = Convert.ToSingle(syringeFlowRateTB.Value.As(new Units.mlPerMinute())) * 1000 / 60.0F;
                var f_actual = qUlPs * t_actual;

                double f_volumeLimit = enableVolumeLimitB.Checked ? Convert.ToDouble(syringeVolumeLimitTB.Value.As(new Units.ul())) : f_actual;
                double t_timeLimit = enableTimeLimit.Checked ? Convert.ToDouble(syringeTimeLimitTB.Value.As(new Units.seconds())) : t_actual;

                double f_timeLimit = qUlPs * t_timeLimit;
                double t_volumeLimit = f_volumeLimit / qUlPs;

                if (t_actual <= t_volumeLimit && t_actual <= t_timeLimit) // not limited by any
                {
                }
                else if (t_volumeLimit <= t_timeLimit) // volume limit
                {
                    sb.AppendLine("Time: " + new TimeSpan(0, 0, 0, (int)t_volumeLimit).ToString(@"hh\:mm\:ss"));
                    sb.AppendLine("Material volume: " + new Quantity(f_volumeLimit, new Units.ul(), false).As(new Units.cc(), "F2") + " cc");

                    sb.AppendLine("\r\nThe pumping will stop when the set volume limit is reached");
                }
                else
                {
                    sb.AppendLine("Time: " + new TimeSpan(0, 0, 0, (int)t_timeLimit).ToString(@"hh\:mm\:ss"));
                    sb.AppendLine("Material volume: " + new Quantity(f_timeLimit, new Units.ul(), false).As(new Units.cc(), "F2") + " cc");

                    sb.AppendLine("\r\nThe pumping will stop when the set time limit is reached");
                }
                if (sb.Length > 0)
                    MessageBox.Show("Please note that these values are just estimates and may vary slightly depending upon the health of the ESS system.\r\n" + sb.ToString(), "Estimates", MessageBoxButtons.OK);
                else
                    MessageBox.Show("To get an estimated time of operation, a time or volume limit must be set.");
            }
        }

        private void VolumePumpedTimer_Tick(object sender, System.EventArgs e)
        {
            var timeMult = 1;
            if (flowrateUS.Value == null)
                return;
            if (flowrateUS.Value.CurrentUnit.Suffix.Split(new char[] { '/' })[1].ToLower()[0] == 'm')
                timeMult = 60;
            else if (flowrateUS.Value.CurrentUnit.Suffix.Split(new char[] { '/' })[1].ToLower()[0] == 'h')
                timeMult = 3600;
            var dt = (DateTime.Now - RasterStartedAt).TotalSeconds;
            materialPumpedL.Text = "Material pumped: " + (double.Parse(syringeFlowRateTB.Text) * dt / timeMult).ToString("F2") + flowrateUS.Value.CurrentUnit.Suffix.Split(new char[] { '/' })[0];
        }

        private void setTempB_Click(object sender, EventArgs e)
        {
            var valC = float.Parse(setTempTB.Value.As(new Units.celsius()));
            float minC = 20;
            float maxC = 200;
            if (valC < minC || valC > maxC)
                MessageBox.Show("Please enter a target temperature within the range [" +
                    new Quantity(minC, new Units.celsius(), false).As(setTempTB.Value.CurrentUnit) +
                    setTempTB.Value.CurrentUnit.Suffix + ", " +
                    new Quantity(maxC, new Units.celsius(), false).As(setTempTB.Value.CurrentUnit) +
                    setTempTB.Value.CurrentUnit.Suffix + "]");
            else
                Machine.SendCom("heat " + setTempTB.Value.As(new Units.celsius()));
        }

        private void setCylenderSpeedB_Click(object sender, EventArgs e)
        {
        }

        private void rasterEnabledCB_CheckedChanged(object sender, EventArgs e)
        {
            if (!((CheckBox)sender).Checked)
                return;

            cylinderP.Visible = false;
            rasterP.Visible = true;

            cylinderEnabledCB.Checked = false;
            noMoveCB.Checked = false;
            new Thread(() => {
                Thread.Sleep(30); Invoke(new MethodInvoker(() => {
                    if (coatB.Text.StartsWith("Begin"))
                        coatB.Text = "Begin " + ProcessString;
                }));
            }).Start();
            File.WriteAllText("programMode.txt", ((CheckBox)sender).Name);
        }

        private void cylinderEnabledCB_CheckedChanged(object sender, EventArgs e)
        {
            if (!((CheckBox)sender).Checked)
                return;
            cylinderP.Visible = true;
            rasterP.Visible = false;

            rasterEnabledCB.Checked = false;
            noMoveCB.Checked = false;
            new Thread(() => {
                Thread.Sleep(30); Invoke(new MethodInvoker(() => {
                    if (coatB.Text.StartsWith("Begin"))
                        coatB.Text = "Begin " + ProcessString;
                }));
            }).Start();
            File.WriteAllText("programMode.txt", ((CheckBox)sender).Name);
        }

        private void noMoveCB_CheckedChanged(object sender, EventArgs e)
        {
            if (!((CheckBox)sender).Checked)
                return;
            cylinderP.Visible = false;
            rasterP.Visible = false;

            rasterEnabledCB.Checked = false;
            cylinderEnabledCB.Checked = false;
            new Thread(() => {
                Thread.Sleep(30); Invoke(new MethodInvoker(() => {
                    if (coatB.Text.StartsWith("Begin"))
                        coatB.Text = "Begin " + ProcessString;
                }));
            }).Start();
            File.WriteAllText("programMode.txt", ((CheckBox)sender).Name);
        }
    }
}
