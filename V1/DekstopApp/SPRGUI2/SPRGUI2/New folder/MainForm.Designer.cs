
namespace SPRGUI2
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.windowedModeB = new MagneticPendulum.Button2();
            this.minimizeB = new MagneticPendulum.Button2();
            this.dataPort = new FivePointNine.Windows.Controls.SerialChannelControl();
            this.closeB = new MagneticPendulum.Button2();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.spModesSC = new System.Windows.Forms.SplitContainer();
            this.spAutoB = new System.Windows.Forms.Button();
            this.pumpBeginB = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.syringeFlowRateTB = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.syringeVolumeLimitTB = new System.Windows.Forms.TextBox();
            this.syringeTimeLimitTB = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.syringeDiaTB = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.enableVolumeLimitB = new FivePointNine.Windows.Controls.FlatCheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.enableTimeLimit = new FivePointNine.Windows.Controls.FlatCheckBox();
            this.spManualB = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.spUpB = new System.Windows.Forms.Button();
            this.spDownB = new System.Windows.Forms.Button();
            this.spDownDownB = new System.Windows.Forms.Button();
            this.spTopB = new System.Windows.Forms.Button();
            this.spUpUpB = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.yCoordL = new System.Windows.Forms.Label();
            this.xCoordL = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.xyStageStatusL = new System.Windows.Forms.Label();
            this.abortB = new System.Windows.Forms.Button();
            this.pumpStatusL = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.rasterBeginB = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.rasterSpeedTB = new System.Windows.Forms.TextBox();
            this.rasterStepSizeTB = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.rasterHeightTB = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.rasterWidthTB = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rasterPad = new SPRGUI2._8DirectionButtonSet();
            this.rasterView1 = new SPRGUI2.RasterView();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spModesSC)).BeginInit();
            this.spModesSC.Panel1.SuspendLayout();
            this.spModesSC.Panel2.SuspendLayout();
            this.spModesSC.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.panel2.Controls.Add(this.label8);
            this.panel2.Location = new System.Drawing.Point(-2, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(857, 22);
            this.panel2.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(0, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(833, 23);
            this.label8.TabIndex = 0;
            this.label8.Text = "SPR Desktop";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label8.DoubleClick += new System.EventHandler(this.label8_DoubleClick);
            this.label8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label8_MouseDown);
            this.label8.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label8_MouseMove);
            this.label8.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label8_MouseUp);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(169, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "Time Trajectory";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(214, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 20);
            this.label5.TabIndex = 2;
            this.label5.Text = "Phase Portrait";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // windowedModeB
            // 
            this.windowedModeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.windowedModeB.BackgroundImage = global::SPRGUI2.Properties.Resources.NormalDim;
            this.windowedModeB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.windowedModeB.FlatAppearance.BorderSize = 0;
            this.windowedModeB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.windowedModeB.HoverBackgroundImage = global::SPRGUI2.Properties.Resources.NormalHighlight;
            this.windowedModeB.Location = new System.Drawing.Point(854, 0);
            this.windowedModeB.Name = "windowedModeB";
            this.windowedModeB.Size = new System.Drawing.Size(22, 22);
            this.windowedModeB.TabIndex = 17;
            this.windowedModeB.UseVisualStyleBackColor = true;
            this.windowedModeB.Click += new System.EventHandler(this.windowedModeB_Click);
            // 
            // minimizeB
            // 
            this.minimizeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizeB.BackgroundImage = global::SPRGUI2.Properties.Resources.MinimizeDim;
            this.minimizeB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.minimizeB.FlatAppearance.BorderSize = 0;
            this.minimizeB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeB.HoverBackgroundImage = global::SPRGUI2.Properties.Resources.CloseHighlighte;
            this.minimizeB.Location = new System.Drawing.Point(832, 0);
            this.minimizeB.Name = "minimizeB";
            this.minimizeB.Size = new System.Drawing.Size(22, 22);
            this.minimizeB.TabIndex = 17;
            this.minimizeB.UseVisualStyleBackColor = true;
            this.minimizeB.Click += new System.EventHandler(this.minimizeB_Click);
            // 
            // dataPort
            // 
            this.dataPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataPort.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.dataPort.DefaultDTR = true;
            this.dataPort.ID = ((byte)(0));
            this.dataPort.Location = new System.Drawing.Point(3, 89);
            this.dataPort.MinimumSize = new System.Drawing.Size(170, 51);
            this.dataPort.MiniPackets = true;
            this.dataPort.Name = "dataPort";
            this.dataPort.Packetize = false;
            this.dataPort.PacketizeData = false;
            this.dataPort.PingDuration = 1000;
            this.dataPort.PingEnabled = false;
            this.dataPort.ReceiveEnabled = true;
            this.dataPort.ShowDTR = false;
            this.dataPort.Size = new System.Drawing.Size(185, 51);
            this.dataPort.TabIndex = 23;
            this.dataPort.Connected += new System.EventHandler(this.dataPort_Connected);
            this.dataPort.Disconnected += new System.EventHandler(this.dataPort_Disconnected);
            // 
            // closeB
            // 
            this.closeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeB.BackgroundImage = global::SPRGUI2.Properties.Resources.CloseDim;
            this.closeB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.closeB.FlatAppearance.BorderSize = 0;
            this.closeB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeB.HoverBackgroundImage = global::SPRGUI2.Properties.Resources.CloseHighlighted;
            this.closeB.Location = new System.Drawing.Point(876, 0);
            this.closeB.Name = "closeB";
            this.closeB.Size = new System.Drawing.Size(22, 22);
            this.closeB.TabIndex = 16;
            this.closeB.UseVisualStyleBackColor = true;
            this.closeB.Click += new System.EventHandler(this.closeB_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(1, 22);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.splitContainer1.Panel1.Controls.Add(this.spModesSC);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.splitContainer1.Panel2.Controls.Add(this.rasterView1);
            this.splitContainer1.Panel2.Controls.Add(this.yCoordL);
            this.splitContainer1.Panel2.Controls.Add(this.xCoordL);
            this.splitContainer1.Panel2.Controls.Add(this.label29);
            this.splitContainer1.Panel2.Controls.Add(this.label28);
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2.Controls.Add(this.rasterBeginB);
            this.splitContainer1.Panel2.Controls.Add(this.rasterPad);
            this.splitContainer1.Panel2.Controls.Add(this.panel5);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(897, 610);
            this.splitContainer1.SplitterDistance = 439;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 24;
            // 
            // spModesSC
            // 
            this.spModesSC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spModesSC.Location = new System.Drawing.Point(1, 64);
            this.spModesSC.Name = "spModesSC";
            this.spModesSC.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spModesSC.Panel1
            // 
            this.spModesSC.Panel1.Controls.Add(this.spAutoB);
            this.spModesSC.Panel1.Controls.Add(this.pumpBeginB);
            this.spModesSC.Panel1.Controls.Add(this.panel1);
            // 
            // spModesSC.Panel2
            // 
            this.spModesSC.Panel2.Controls.Add(this.spManualB);
            this.spModesSC.Panel2.Controls.Add(this.panel3);
            this.spModesSC.Size = new System.Drawing.Size(439, 534);
            this.spModesSC.SplitterDistance = 359;
            this.spModesSC.TabIndex = 1;
            // 
            // spAutoB
            // 
            this.spAutoB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spAutoB.BackColor = System.Drawing.Color.Gainsboro;
            this.spAutoB.FlatAppearance.BorderSize = 0;
            this.spAutoB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spAutoB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spAutoB.Location = new System.Drawing.Point(0, 0);
            this.spAutoB.Name = "spAutoB";
            this.spAutoB.Size = new System.Drawing.Size(439, 28);
            this.spAutoB.TabIndex = 2;
            this.spAutoB.Text = "Automatic Operation";
            this.spAutoB.UseVisualStyleBackColor = false;
            this.spAutoB.Click += new System.EventHandler(this.spAutoB_Click);
            // 
            // pumpBeginB
            // 
            this.pumpBeginB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pumpBeginB.BackColor = System.Drawing.Color.Silver;
            this.pumpBeginB.FlatAppearance.BorderSize = 0;
            this.pumpBeginB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pumpBeginB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.pumpBeginB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pumpBeginB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pumpBeginB.Location = new System.Drawing.Point(102, 271);
            this.pumpBeginB.Name = "pumpBeginB";
            this.pumpBeginB.Size = new System.Drawing.Size(215, 31);
            this.pumpBeginB.TabIndex = 1;
            this.pumpBeginB.Text = "Begin";
            this.pumpBeginB.UseVisualStyleBackColor = false;
            this.pumpBeginB.Click += new System.EventHandler(this.pumpBeginB_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.syringeFlowRateTB);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.syringeVolumeLimitTB);
            this.panel1.Controls.Add(this.syringeTimeLimitTB);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.syringeDiaTB);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.enableVolumeLimitB);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.enableTimeLimit);
            this.panel1.Location = new System.Drawing.Point(79, 94);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(265, 135);
            this.panel1.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(3, 1);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "Syringe Diameter";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // syringeFlowRateTB
            // 
            this.syringeFlowRateTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.syringeFlowRateTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.syringeFlowRateTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syringeFlowRateTB.Location = new System.Drawing.Point(140, 38);
            this.syringeFlowRateTB.Name = "syringeFlowRateTB";
            this.syringeFlowRateTB.Size = new System.Drawing.Size(48, 19);
            this.syringeFlowRateTB.TabIndex = 3;
            this.syringeFlowRateTB.Text = "1";
            this.syringeFlowRateTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(194, 2);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 20);
            this.label10.TabIndex = 0;
            this.label10.Text = "mm";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // syringeVolumeLimitTB
            // 
            this.syringeVolumeLimitTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.syringeVolumeLimitTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.syringeVolumeLimitTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syringeVolumeLimitTB.Location = new System.Drawing.Point(140, 109);
            this.syringeVolumeLimitTB.Name = "syringeVolumeLimitTB";
            this.syringeVolumeLimitTB.Size = new System.Drawing.Size(48, 19);
            this.syringeVolumeLimitTB.TabIndex = 3;
            this.syringeVolumeLimitTB.Text = "1";
            this.syringeVolumeLimitTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // syringeTimeLimitTB
            // 
            this.syringeTimeLimitTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.syringeTimeLimitTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.syringeTimeLimitTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syringeTimeLimitTB.Location = new System.Drawing.Point(140, 74);
            this.syringeTimeLimitTB.Name = "syringeTimeLimitTB";
            this.syringeTimeLimitTB.Size = new System.Drawing.Size(48, 19);
            this.syringeTimeLimitTB.TabIndex = 3;
            this.syringeTimeLimitTB.Text = "1";
            this.syringeTimeLimitTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(53, 37);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(81, 20);
            this.label13.TabIndex = 0;
            this.label13.Text = "Flow Rate";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // syringeDiaTB
            // 
            this.syringeDiaTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.syringeDiaTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.syringeDiaTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syringeDiaTB.Location = new System.Drawing.Point(140, 2);
            this.syringeDiaTB.Name = "syringeDiaTB";
            this.syringeDiaTB.Size = new System.Drawing.Size(48, 19);
            this.syringeDiaTB.TabIndex = 3;
            this.syringeDiaTB.Text = "1";
            this.syringeDiaTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(194, 38);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(33, 20);
            this.label14.TabIndex = 0;
            this.label14.Text = "µl/h";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(194, 74);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 20);
            this.label15.TabIndex = 0;
            this.label15.Text = "minutes";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // enableVolumeLimitB
            // 
            this.enableVolumeLimitB.CheckedColor = System.Drawing.Color.Black;
            this.enableVolumeLimitB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enableVolumeLimitB.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.enableVolumeLimitB.Location = new System.Drawing.Point(11, 108);
            this.enableVolumeLimitB.Name = "enableVolumeLimitB";
            this.enableVolumeLimitB.Size = new System.Drawing.Size(123, 24);
            this.enableVolumeLimitB.TabIndex = 3;
            this.enableVolumeLimitB.Text = "Volume Limit";
            this.enableVolumeLimitB.UncheckedColor = System.Drawing.Color.DimGray;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(194, 109);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(20, 20);
            this.label16.TabIndex = 0;
            this.label16.Text = "µl";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // enableTimeLimit
            // 
            this.enableTimeLimit.CheckedColor = System.Drawing.Color.Black;
            this.enableTimeLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enableTimeLimit.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.enableTimeLimit.Location = new System.Drawing.Point(30, 73);
            this.enableTimeLimit.Name = "enableTimeLimit";
            this.enableTimeLimit.Size = new System.Drawing.Size(104, 24);
            this.enableTimeLimit.TabIndex = 3;
            this.enableTimeLimit.Text = "Time Limit";
            this.enableTimeLimit.UncheckedColor = System.Drawing.Color.DimGray;
            // 
            // spManualB
            // 
            this.spManualB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spManualB.BackColor = System.Drawing.Color.Gainsboro;
            this.spManualB.FlatAppearance.BorderSize = 0;
            this.spManualB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spManualB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spManualB.Location = new System.Drawing.Point(0, 0);
            this.spManualB.Name = "spManualB";
            this.spManualB.Size = new System.Drawing.Size(439, 28);
            this.spManualB.TabIndex = 2;
            this.spManualB.Text = "Manual Movement";
            this.spManualB.UseVisualStyleBackColor = false;
            this.spManualB.Click += new System.EventHandler(this.spManualB_Click);
            // 
            // panel3
            // 
            this.panel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.spUpB);
            this.panel3.Controls.Add(this.spDownB);
            this.panel3.Controls.Add(this.spDownDownB);
            this.panel3.Controls.Add(this.spTopB);
            this.panel3.Controls.Add(this.spUpUpB);
            this.panel3.Location = new System.Drawing.Point(38, 58);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(356, 68);
            this.panel3.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label7.Location = new System.Drawing.Point(10, 2);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 62);
            this.label7.TabIndex = 0;
            this.label7.Text = "Move the plunger";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // spUpB
            // 
            this.spUpB.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.spUpB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.spUpB.FlatAppearance.BorderSize = 0;
            this.spUpB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spUpB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.spUpB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spUpB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.spUpB.Location = new System.Drawing.Point(105, 2);
            this.spUpB.Name = "spUpB";
            this.spUpB.Size = new System.Drawing.Size(82, 31);
            this.spUpB.TabIndex = 1;
            this.spUpB.Text = "+";
            this.spUpB.UseVisualStyleBackColor = false;
            this.spUpB.Click += new System.EventHandler(this.spUpB_Click);
            // 
            // spDownB
            // 
            this.spDownB.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.spDownB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.spDownB.FlatAppearance.BorderSize = 0;
            this.spDownB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spDownB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.spDownB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spDownB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.spDownB.Location = new System.Drawing.Point(105, 34);
            this.spDownB.Name = "spDownB";
            this.spDownB.Size = new System.Drawing.Size(82, 31);
            this.spDownB.TabIndex = 1;
            this.spDownB.Text = "-";
            this.spDownB.UseVisualStyleBackColor = false;
            this.spDownB.Click += new System.EventHandler(this.spDownB_Click);
            // 
            // spDownDownB
            // 
            this.spDownDownB.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.spDownDownB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.spDownDownB.FlatAppearance.BorderSize = 0;
            this.spDownDownB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spDownDownB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.spDownDownB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spDownDownB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.spDownDownB.Location = new System.Drawing.Point(188, 34);
            this.spDownDownB.Name = "spDownDownB";
            this.spDownDownB.Size = new System.Drawing.Size(82, 31);
            this.spDownDownB.TabIndex = 1;
            this.spDownDownB.Text = "--";
            this.spDownDownB.UseVisualStyleBackColor = false;
            this.spDownDownB.Click += new System.EventHandler(this.spDownDownB_Click);
            // 
            // spTopB
            // 
            this.spTopB.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.spTopB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.spTopB.FlatAppearance.BorderSize = 0;
            this.spTopB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spTopB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.spTopB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spTopB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.spTopB.Location = new System.Drawing.Point(271, 2);
            this.spTopB.Name = "spTopB";
            this.spTopB.Size = new System.Drawing.Size(82, 31);
            this.spTopB.TabIndex = 1;
            this.spTopB.Text = "Top";
            this.spTopB.UseVisualStyleBackColor = false;
            this.spTopB.Click += new System.EventHandler(this.spTopB_Click);
            // 
            // spUpUpB
            // 
            this.spUpUpB.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.spUpUpB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.spUpUpB.FlatAppearance.BorderSize = 0;
            this.spUpUpB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spUpUpB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.spUpUpB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spUpUpB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.spUpUpB.Location = new System.Drawing.Point(188, 2);
            this.spUpUpB.Name = "spUpUpB";
            this.spUpUpB.Size = new System.Drawing.Size(82, 31);
            this.spUpUpB.TabIndex = 1;
            this.spUpUpB.Text = "++";
            this.spUpUpB.UseVisualStyleBackColor = false;
            this.spUpUpB.Click += new System.EventHandler(this.spUpUpB_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(429, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Syringe Pump";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // yCoordL
            // 
            this.yCoordL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.yCoordL.AutoSize = true;
            this.yCoordL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yCoordL.Location = new System.Drawing.Point(374, 254);
            this.yCoordL.Name = "yCoordL";
            this.yCoordL.Size = new System.Drawing.Size(14, 20);
            this.yCoordL.TabIndex = 0;
            this.yCoordL.Text = "-";
            this.yCoordL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // xCoordL
            // 
            this.xCoordL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.xCoordL.AutoSize = true;
            this.xCoordL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xCoordL.Location = new System.Drawing.Point(276, 254);
            this.xCoordL.Name = "xCoordL";
            this.xCoordL.Size = new System.Drawing.Size(14, 20);
            this.xCoordL.TabIndex = 0;
            this.xCoordL.Text = "-";
            this.xCoordL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            this.label29.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(348, 254);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(20, 20);
            this.label29.TabIndex = 0;
            this.label29.Text = "Y";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label28
            // 
            this.label28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(250, 254);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(20, 20);
            this.label28.TabIndex = 0;
            this.label28.Text = "X";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.Silver;
            this.panel4.Controls.Add(this.xyStageStatusL);
            this.panel4.Controls.Add(this.abortB);
            this.panel4.Controls.Add(this.pumpStatusL);
            this.panel4.Controls.Add(this.label24);
            this.panel4.Controls.Add(this.dataPort);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.label17);
            this.panel4.Location = new System.Drawing.Point(6, 467);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(441, 143);
            this.panel4.TabIndex = 2;
            // 
            // xyStageStatusL
            // 
            this.xyStageStatusL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.xyStageStatusL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.xyStageStatusL.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xyStageStatusL.Location = new System.Drawing.Point(320, 45);
            this.xyStageStatusL.Name = "xyStageStatusL";
            this.xyStageStatusL.Size = new System.Drawing.Size(114, 24);
            this.xyStageStatusL.TabIndex = 0;
            this.xyStageStatusL.Text = "--";
            this.xyStageStatusL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // abortB
            // 
            this.abortB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.abortB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.abortB.Enabled = false;
            this.abortB.FlatAppearance.BorderSize = 0;
            this.abortB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.abortB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.abortB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.abortB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.abortB.Location = new System.Drawing.Point(331, 89);
            this.abortB.Name = "abortB";
            this.abortB.Size = new System.Drawing.Size(104, 51);
            this.abortB.TabIndex = 1;
            this.abortB.Text = "Abort";
            this.abortB.UseVisualStyleBackColor = false;
            this.abortB.Click += new System.EventHandler(this.abortB_Click);
            // 
            // pumpStatusL
            // 
            this.pumpStatusL.BackColor = System.Drawing.Color.DimGray;
            this.pumpStatusL.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pumpStatusL.Location = new System.Drawing.Point(78, 45);
            this.pumpStatusL.Name = "pumpStatusL";
            this.pumpStatusL.Size = new System.Drawing.Size(114, 24);
            this.pumpStatusL.TabIndex = 0;
            this.pumpStatusL.Text = "OFF";
            this.pumpStatusL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(229, 45);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(89, 24);
            this.label24.TabIndex = 0;
            this.label24.Text = "XY Stage";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 24);
            this.label3.TabIndex = 0;
            this.label3.Text = "Pump";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(10, 4);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(424, 35);
            this.label17.TabIndex = 0;
            this.label17.Text = "System";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label17.Click += new System.EventHandler(this.label2_Click);
            // 
            // rasterBeginB
            // 
            this.rasterBeginB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rasterBeginB.BackColor = System.Drawing.Color.Silver;
            this.rasterBeginB.FlatAppearance.BorderSize = 0;
            this.rasterBeginB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rasterBeginB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.rasterBeginB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rasterBeginB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.rasterBeginB.Location = new System.Drawing.Point(136, 421);
            this.rasterBeginB.Name = "rasterBeginB";
            this.rasterBeginB.Size = new System.Drawing.Size(195, 31);
            this.rasterBeginB.TabIndex = 1;
            this.rasterBeginB.Text = "Begin";
            this.rasterBeginB.UseVisualStyleBackColor = false;
            this.rasterBeginB.Click += new System.EventHandler(this.rasterBeginB_Click);
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.Controls.Add(this.label18);
            this.panel5.Controls.Add(this.rasterSpeedTB);
            this.panel5.Controls.Add(this.rasterStepSizeTB);
            this.panel5.Controls.Add(this.label19);
            this.panel5.Controls.Add(this.rasterHeightTB);
            this.panel5.Controls.Add(this.label20);
            this.panel5.Controls.Add(this.label21);
            this.panel5.Controls.Add(this.label27);
            this.panel5.Controls.Add(this.label22);
            this.panel5.Controls.Add(this.rasterWidthTB);
            this.panel5.Controls.Add(this.label26);
            this.panel5.Controls.Add(this.label23);
            this.panel5.Location = new System.Drawing.Point(6, 343);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(441, 72);
            this.panel5.TabIndex = 4;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(11, 7);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 20);
            this.label18.TabIndex = 0;
            this.label18.Text = "Width";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rasterSpeedTB
            // 
            this.rasterSpeedTB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rasterSpeedTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.rasterSpeedTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rasterSpeedTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rasterSpeedTB.Location = new System.Drawing.Point(328, 42);
            this.rasterSpeedTB.Name = "rasterSpeedTB";
            this.rasterSpeedTB.Size = new System.Drawing.Size(48, 19);
            this.rasterSpeedTB.TabIndex = 3;
            this.rasterSpeedTB.Text = "1";
            this.rasterSpeedTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.rasterSpeedTB.TextChanged += RasterParamTB_TextChanged;
            // 
            // rasterStepSizeTB
            // 
            this.rasterStepSizeTB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rasterStepSizeTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.rasterStepSizeTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rasterStepSizeTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rasterStepSizeTB.Location = new System.Drawing.Point(328, 7);
            this.rasterStepSizeTB.Name = "rasterStepSizeTB";
            this.rasterStepSizeTB.Size = new System.Drawing.Size(48, 19);
            this.rasterStepSizeTB.TabIndex = 3;
            this.rasterStepSizeTB.Text = "1";
            this.rasterStepSizeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.rasterStepSizeTB.TextChanged += RasterParamTB_TextChanged;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(121, 7);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(35, 20);
            this.label19.TabIndex = 0;
            this.label19.Text = "mm";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rasterHeightTB
            // 
            this.rasterHeightTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.rasterHeightTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rasterHeightTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rasterHeightTB.Location = new System.Drawing.Point(67, 42);
            this.rasterHeightTB.Name = "rasterHeightTB";
            this.rasterHeightTB.Size = new System.Drawing.Size(48, 19);
            this.rasterHeightTB.TabIndex = 3;
            this.rasterHeightTB.Text = "1";
            this.rasterHeightTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.rasterHeightTB.TextChanged += RasterParamTB_TextChanged;
            // 

            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(5, 42);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(56, 20);
            this.label20.TabIndex = 0;
            this.label20.Text = "Height";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(121, 42);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(35, 20);
            this.label21.TabIndex = 0;
            this.label21.Text = "mm";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label27
            // 
            this.label27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(266, 42);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(56, 20);
            this.label27.TabIndex = 0;
            this.label27.Text = "Speed";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(244, 7);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(78, 20);
            this.label22.TabIndex = 0;
            this.label22.Text = "Step Size";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rasterWidthTB
            // 
            this.rasterWidthTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.rasterWidthTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rasterWidthTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rasterWidthTB.Location = new System.Drawing.Point(67, 7);
            this.rasterWidthTB.Name = "rasterWidthTB";
            this.rasterWidthTB.Size = new System.Drawing.Size(48, 19);
            this.rasterWidthTB.TabIndex = 3;
            this.rasterWidthTB.Text = "1";
            this.rasterWidthTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.rasterWidthTB.TextChanged += RasterParamTB_TextChanged;
            // 
            // label26
            // 
            this.label26.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(382, 42);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(47, 20);
            this.label26.TabIndex = 0;
            this.label26.Text = "mm/s";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label23
            // 
            this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(382, 7);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(35, 20);
            this.label23.TabIndex = 0;
            this.label23.Text = "mm";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(67, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(352, 35);
            this.label2.TabIndex = 0;
            this.label2.Text = "Linear Stage";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(67, 309);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(352, 35);
            this.label4.TabIndex = 0;
            this.label4.Text = "Raster Movement";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Click += new System.EventHandler(this.label2_Click);
            // 
            // rasterPad
            // 
            this.rasterPad.Location = new System.Drawing.Point(9, 51);
            this.rasterPad.Name = "rasterPad";
            this.rasterPad.Size = new System.Drawing.Size(200, 200);
            this.rasterPad.TabIndex = 1;
            this.rasterPad.OnButtonClick += new SPRGUI2._8DirectionButtonSet.ButtonClickHandler(this._8DirectionButtonSet1_OnButtonClick);
            // 
            // rasterView1
            // 
            this.rasterView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rasterView1.Location = new System.Drawing.Point(239, 51);
            this.rasterView1.Name = "rasterView1";
            this.rasterView1.Size = new System.Drawing.Size(200, 200);
            this.rasterView1.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.ClientSize = new System.Drawing.Size(898, 632);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.windowedModeB);
            this.Controls.Add(this.minimizeB);
            this.Controls.Add(this.closeB);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(726, 468);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.spModesSC.Panel1.ResumeLayout(false);
            this.spModesSC.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spModesSC)).EndInit();
            this.spModesSC.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;    
        private MagneticPendulum.Button2 closeB;
        private MagneticPendulum.Button2 minimizeB;
        private FivePointNine.Windows.Controls.SerialChannelControl dataPort;
        private MagneticPendulum.Button2 windowedModeB;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label pumpStatusL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer spModesSC;
        private System.Windows.Forms.TextBox syringeFlowRateTB;
        private System.Windows.Forms.TextBox syringeDiaTB;
        private System.Windows.Forms.Button spAutoB;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button spManualB;
        private System.Windows.Forms.Button spDownB;
        private System.Windows.Forms.Button spTopB;
        private System.Windows.Forms.Button spUpUpB;
        private System.Windows.Forms.Button spDownDownB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button spUpB;
        private FivePointNine.Windows.Controls.FlatCheckBox enableTimeLimit;
        private System.Windows.Forms.TextBox syringeVolumeLimitTB;
        private System.Windows.Forms.TextBox syringeTimeLimitTB;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private FivePointNine.Windows.Controls.FlatCheckBox enableVolumeLimitB;
        private System.Windows.Forms.Button pumpBeginB;
        private _8DirectionButtonSet rasterPad;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button abortB;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox rasterStepSizeTB;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox rasterHeightTB;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox rasterWidthTB;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button rasterBeginB;
        private System.Windows.Forms.TextBox rasterSpeedTB;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label yCoordL;
        private System.Windows.Forms.Label xCoordL;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label xyStageStatusL;
        private System.Windows.Forms.Label label4;
        private RasterView rasterView1;
    }
}

