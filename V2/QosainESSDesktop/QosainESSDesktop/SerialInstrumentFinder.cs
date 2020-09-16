using System;
using System.Collections.Generic;
using System.Management;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;

namespace QosainESSDesktop
{
    public partial class SerialInstrumentFinder : Form
    {
        public SerialInstrumentFinder()
        {
            InitializeComponent();

            new Thread(() => { refresh(); instrumentWatcherThread(); }).Start();
        }
        SerialDevice Selected = null;

        public string DeviceToLookFor { get; set; } = "";
        public int BaudRate { get; set; } = 115200;
        public int Delay { get; set; } = 2000;
        public bool ClosePortAfterChecking { get; private set; }
        public bool DTR { get; set; } = true;
        public class SerialDevice
        {
            public event EventHandler OnResult;
            public string USBDeviceName { get; set; }
            public string ComPortName { get; set; }
            public int BaudRate { get; set; } = 0;
            public bool DTR { get; set; } = false;
            public int Delay { get; set; } = 0;
            public string InstrumentName { get; set; } = "";
            Thread nameFinder;
            public SerialPort SerialPort { get; private set; }
            public bool DoneFinding = false;
            public bool ClosePortAfterChecking { get; set; }

            public void FindInstrumentName()
            {
                nameFinder = new Thread(() =>
                {
                    SerialPort = new SerialPort(ComPortName, BaudRate);
                    SerialPort.DtrEnable = DTR;
                    SerialPort.Encoding = new UTF8Encoding();
                    for (int mainRetries = 0; mainRetries < 10 && InstrumentName == ""; mainRetries++)
                    {
                        try
                        {
                            SerialPort.Open();
                        }
                        catch { Thread.Sleep(1000); continue; }
                        Thread.Sleep(Delay);
                        bool headerReceived = false;
                        for (int retries = 0; retries < 3 && !headerReceived; retries++)
                        {
                            var before = SerialPort.ReadExisting();
                            var call = new byte[] { (byte)253, (byte)':' };
                            SerialPort.Write(call, 0, 2);
                            string str = "";
                            var start = DateTime.Now;
                            var header = new byte[] { 0xAA, 0xFF, 253, (byte)':' };
                            while ((DateTime.Now - start).TotalMilliseconds < 1000)
                            {
                                if (SerialPort.BytesToRead > 4)
                                {
                                    bool allGood = true;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        var br = new byte[] { 0 };
                                        SerialPort.Read(br, 0, 1);
                                        if (br[0] != header[i])
                                        {
                                            str += ((char)br[0]).ToString();
                                            allGood = false;
                                            break;
                                        }
                                        else
                                        { }
                                    }
                                    if (!allGood)
                                        continue;
                                    else
                                    {
                                        headerReceived = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (headerReceived)
                        {
                            var start = DateTime.Now;
                            while (SerialPort.BytesToRead < 1 && (DateTime.Now - start).TotalMilliseconds < 1000)
                                Thread.Sleep(30);
                            if (SerialPort.BytesToRead >= 1)
                            {
                                int nameLength = SerialPort.ReadByte();
                                while (SerialPort.BytesToRead < nameLength && (DateTime.Now - start).TotalMilliseconds < 1000)
                                    Thread.Sleep(30);
                                if (SerialPort.BytesToRead >= nameLength)
                                {
                                    var buf = new byte[nameLength];
                                    SerialPort.Read(buf, 0, nameLength);
                                    InstrumentName = new UTF8Encoding().GetString(buf);
                                    if (ClosePortAfterChecking)
                                        SerialPort.Close();
                                    nameFinder = null;
                                    DoneFinding = true;
                                    OnResult?.Invoke(this, new EventArgs());
                                    return;
                                }
                            }
                        }
                    }
                    SerialPort.Close(); 
                    SerialPort = null; nameFinder = null;
                    OnResult?.Invoke(this, new EventArgs());
                    DoneFinding = true;

                });
                nameFinder.Start();
            }
            public void NotifyRemoved()
            {
                if (nameFinder != null)
                    nameFinder.Abort();
                nameFinder = null;
                if (SerialPort != null)
                {
                    if (SerialPort.IsOpen)
                        SerialPort.Close();
                }
            }
        }
        List<SerialDevice> allDevices = new List<SerialDevice>();
        bool inRefresh = false;
        void refresh()
        {
            while (inRefresh) Thread.Sleep(30);
            new Thread(() =>
            {
                inRefresh = true;
                ManagementClass processClass = new ManagementClass("Win32_PnPEntity");
                ManagementObjectCollection Ports = processClass.GetInstances();
                var usbDeviceNames = new Dictionary<string, string>();
                foreach (ManagementObject property in Ports)
                {
                    if (property.GetPropertyValue("Name") != null)
                        if (property.GetPropertyValue("Name").ToString().Contains("USB") &&
                            property.GetPropertyValue("Name").ToString().Contains("COM"))
                        {
                            var fullName = property.GetPropertyValue("Name").ToString();
                            foreach (var port in SerialPort.GetPortNames())
                            {
                                if (fullName.ToLower().Contains(port.ToLower()))
                                    usbDeviceNames.Add(fullName, port);
                            }
                        }

                }
                var toRemove = new List<SerialDevice>();
                var toAdd = new List<SerialDevice>();
                foreach (var sDevice in allDevices)
                {
                    if (usbDeviceNames.ContainsKey(sDevice.USBDeviceName))
                        // is alive
                        continue;
                    toRemove.Add(sDevice);
                }

                foreach (var usbDev in usbDeviceNames)
                {
                    if (allDevices.Find(ad => ad.USBDeviceName == usbDev.Key && ad.ComPortName == usbDev.Value) != null)
                        // is already there
                        continue;
                    //var listItem
                    var newDevice = new SerialDevice()
                    {
                        ComPortName = usbDev.Value,
                        USBDeviceName = usbDev.Key,
                        BaudRate = this.BaudRate,
                        DTR = this.DTR,
                        Delay = Delay,
                        ClosePortAfterChecking = this.ClosePortAfterChecking
                    };
                    toAdd.Add(newDevice);
                }
                foreach (var nd in toAdd)
                {
                    var instC = new InstrumentFinderListItem()
                    {
                        LoadingAnimImagesDirectory = "loadingAnim",
                        LoadingAnimImagesKey = "Spinner-1s-200px-*.jpg",
                        Width = flowLayoutPanel1.Width,
                        Device = nd
                    };
                    this.FormClosing += (s, e) => { instC.ExitRequest = true; };
                    instC.OnSelected += (s, e) =>
                    {
                        Selected = nd;
                        Close();
                    };
                    nd.OnResult += (s, e) =>
                    {
                        this.Invoke(new MethodInvoker(() =>
                        {
                            if (nd.InstrumentName == "") // not found
                            {
                                instC.SetText("{" + "Unknown Device" + "}", false);
                                instC.SetWorking(false);
                            }
                            else
                            {
                                instC.SetText(nd.InstrumentName, nd.InstrumentName == DeviceToLookFor || DeviceToLookFor == "");
                                instC.SetWorking(false);
                            }
                        }));
                        foreach (var d in allDevices)
                        {
                            if (d.DoneFinding)
                                return;
                        }
                        this.Invoke(new MethodInvoker(() => { label1.Text = "Select a device"; }));
                    };
                    if (allDevices.Find(d => d.USBDeviceName == nd.USBDeviceName) != null)
                    { continue; }
                    allDevices.Add(nd);
                    this.Invoke(new MethodInvoker(() =>
                    {
                        if (flowLayoutPanel1.Controls.Contains(noDevicesL))
                            flowLayoutPanel1.Controls.Remove(noDevicesL);
                        flowLayoutPanel1.Controls.Add(instC);
                        instC.SetText("{" + nd.USBDeviceName + "}", false);
                    }));
                    nd.FindInstrumentName();
                }
                foreach (var rd in toRemove)
                {
                    allDevices.Remove(rd);
                    rd.NotifyRemoved();
                    Invoke(new MethodInvoker(() =>
                    {
                        flowLayoutPanel1.Controls.Remove(flowLayoutPanel1.Controls.OfType<InstrumentFinderListItem>().ToList().Find(c => c.Device == rd));
                        if (flowLayoutPanel1.Controls.Count == 0)
                            flowLayoutPanel1.Controls.Add(noDevicesL);
                    }));
                }
                inRefresh = false;
            }).Start();
        }
        private void OnDevicesChanged(object sender, EventArrivedEventArgs e)
        {
            refresh();
        }

        bool exitRequest = false;
        private void instrumentWatcherThread()
        {
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");

            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(OnDevicesChanged);
            insertWatcher.Start();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(OnDevicesChanged);
            removeWatcher.Start();

            while (!exitRequest)
                // Do something while waiting for events
                System.Threading.Thread.Sleep(30);
            removeWatcher.EventArrived -= OnDevicesChanged;
            insertWatcher.EventArrived -= OnDevicesChanged;
            insertWatcher.Dispose();
            removeWatcher.Dispose();
            foreach (var d in allDevices)
            {
                if (d == Selected) continue;
                d.NotifyRemoved();
            }
        }

        public static SerialDevice GetDevice(string nameKey = "", int baud = 115200, bool dtr = true, int delay = 2000, bool closePortAfterChecking = false)
        {
            var f = new SerialInstrumentFinder();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.DeviceToLookFor = nameKey;
            f.BaudRate = baud;
            f.DTR = dtr;
            f.Delay = delay;
            f.ClosePortAfterChecking = closePortAfterChecking;
            try
            {
                Application.Run(f);
            }
            catch(InvalidOperationException ex) { f.ShowDialog(); }
            return f.Selected;
        }
        private void OpenInstrumentUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            exitRequest = true;
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            noDevicesL.Width = flowLayoutPanel1.Width;
            noDevicesL.Height = flowLayoutPanel1.Height;
        }
    }
}