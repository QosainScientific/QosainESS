using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using static QosainESSDesktop.SerialInstrumentFinder;

namespace QosainESSDesktop
{
    public partial class InstrumentFinderListItem : UserControl
    {
        public bool ExitRequest { get; set; }
        public SerialDevice Device { get; set; }
        public InstrumentFinderListItem()
        {
            InitializeComponent();
            new Thread(() =>
            {
                while (!ExitRequest)
                {
                    Thread.Sleep(100);
                    if (images == null)
                    {
                        images = new Bitmap[Directory.GetFiles(LoadingAnimImagesDirectory, LoadingAnimImagesKey).Length];
                        new Thread(() =>
                        {
                            var files = Directory.GetFiles(LoadingAnimImagesDirectory, LoadingAnimImagesKey);
                            images = files.Select(f => new Bitmap(Image.FromFile(f))).ToArray();
                        }).Start();
                    }
                    if (images != null)
                    {
                        index++;
                        index = index % images.Length;
                        panel1.BackgroundImage = images[index];
                    }
                }
            }).Start();

        }
        public void SetText(string text, bool enabled)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                button1.Text = text;
                button1.Enabled = enabled;
            }));
        }
        public event EventHandler OnSelected;
        public void SetWorking(bool working)
        {
            panel1.Visible = working;
        }

        Bitmap[] images;
        public string LoadingAnimImagesDirectory { get; set; } = "";
        public string LoadingAnimImagesKey { get; set; } = "f (*).jpg";
        int index = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OnSelected?.Invoke(this, new EventArgs());
        }
    }
}
