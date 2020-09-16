namespace SPRGUI2
{
    partial class PlainProgressBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressL = new System.Windows.Forms.Label();
            this.percentL = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.startedL = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.elapsedL = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.remainingL = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(3, 59);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(326, 23);
            this.progressBar1.TabIndex = 0;
            this.progressBar1.Value = 50;
            // 
            // progressL
            // 
            this.progressL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressL.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressL.Location = new System.Drawing.Point(324, 34);
            this.progressL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.progressL.Name = "progressL";
            this.progressL.Size = new System.Drawing.Size(87, 54);
            this.progressL.TabIndex = 1;
            this.progressL.Text = "49";
            this.progressL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // percentL
            // 
            this.percentL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.percentL.AutoSize = true;
            this.percentL.Location = new System.Drawing.Point(399, 58);
            this.percentL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.percentL.Name = "percentL";
            this.percentL.Size = new System.Drawing.Size(23, 20);
            this.percentL.TabIndex = 2;
            this.percentL.Text = "%";
            this.percentL.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Started:";
            // 
            // startedL
            // 
            this.startedL.AutoSize = true;
            this.startedL.Location = new System.Drawing.Point(104, 3);
            this.startedL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.startedL.Name = "startedL";
            this.startedL.Size = new System.Drawing.Size(71, 20);
            this.startedL.TabIndex = 3;
            this.startedL.Text = "02:32:12";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(243, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Elapsed:";
            // 
            // elapsedL
            // 
            this.elapsedL.AutoSize = true;
            this.elapsedL.Location = new System.Drawing.Point(325, 3);
            this.elapsedL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.elapsedL.Name = "elapsedL";
            this.elapsedL.Size = new System.Drawing.Size(71, 20);
            this.elapsedL.TabIndex = 3;
            this.elapsedL.Text = "00:12:01";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 23);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "Remaining:";
            // 
            // remainingL
            // 
            this.remainingL.AutoSize = true;
            this.remainingL.Location = new System.Drawing.Point(104, 23);
            this.remainingL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.remainingL.Name = "remainingL";
            this.remainingL.Size = new System.Drawing.Size(62, 20);
            this.remainingL.TabIndex = 3;
            this.remainingL.Text = "4:49:06";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PlainProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.remainingL);
            this.Controls.Add(this.elapsedL);
            this.Controls.Add(this.startedL);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.percentL);
            this.Controls.Add(this.progressL);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PlainProgressBar";
            this.Size = new System.Drawing.Size(422, 83);
            this.Load += new System.EventHandler(this.PlainProgressBar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label progressL;
        private System.Windows.Forms.Label percentL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label startedL;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label elapsedL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label remainingL;
        private System.Windows.Forms.Timer timer1;
    }
}
