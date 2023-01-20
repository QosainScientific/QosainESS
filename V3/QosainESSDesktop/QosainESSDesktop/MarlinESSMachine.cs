﻿using SharpDX.Direct3D9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace QosainESSDesktop
{
    public delegate void MachineErrorEventHandler(string message);
    public delegate void StatusArgsUpdateEventHandler(string name, Dictionary<string, string> args);
    public class ESSMachine
    {
        public static event MachineErrorEventHandler OnMachineError;
        public event StatusArgsUpdateEventHandler OnStatusArgsUpdate;
        protected bool inTick = false;
        public static void ThrowMachineError(string message)
        {
            OnMachineError?.Invoke(message);
        }
        protected void applyStatusArgs(string name, Dictionary<string, string> args = null)
        {
            if (args == null)
                args = new Dictionary<string, string>();
            OnStatusArgsUpdate?.Invoke(name, args);
        }

        public SerialPort Channel;
        public ESSMachine(SerialPort sp)
        {
            Channel = sp;
        }
        public virtual void SendCom(string com)
        {
            throw new NotImplementedException();
        }
        public virtual string waitForResponse(string com, int timeOut)
        {
            throw new NotImplementedException();
        }
        public virtual void Poller_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        public static Dictionary<string, string> getArgs(string CommandRaw)
        {
            var comParts = CommandRaw.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (comParts.Length == 2)
                comParts = comParts[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            else
                comParts = new string[0];
            Dictionary<string, string> Args = new Dictionary<string, string>();
            for (int i = 0; i < comParts.Length; i++)
            {
                try
                {
                    var pair = comParts[i].Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    Args.Add(pair[0].Trim(), pair[1].Trim());
                }
                catch { }
            }
            return Args;
        }
        public virtual void InPlaceReset(float[] backupXYZ)
        {
            throw new NotFiniteNumberException();
        }
    }
    public class ArduinoESSMachine : ESSMachine
    {
        public ArduinoESSMachine(SerialPort sp) : base(sp)
        {

        }
        public override void SendCom(string com)
        {
            if (Channel == null)
            {
                MessageBox.Show("Kindly connect to the hardware first (E02)");
                return;
            }
            Channel.Write(new UTF8Encoding().GetBytes(com + "\n"), 0, com.Length + 1);
            //sendInParts(com + "\n");
        }
        public override void Poller_Tick(object sender, EventArgs e)
        {
            if (inTick)
                return;
            inTick = true;
            if (Channel == null)
                return;
            if (!Channel.IsOpen)
                return;
            while (Channel.BytesToRead > 0)
            {
                var raw = ReceiveCom(500);
                var name = getCommand(raw);

                var args = getArgs(raw);
                applyStatusArgs(name, args);
            }
            inTick = false;
        }
        public override string waitForResponse(string com, int timeOut)
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
            for (int i = 0; i < timeOut; i += 30)
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
        public string getCommand(string CommandRaw)
        {
            if (CommandRaw == "")
                return "";
            return CommandRaw.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
        }
        public override void InPlaceReset(float[] backupXYZ)
        {
            Channel.Close();
            Channel = new SerialPort(Channel.PortName, Channel.BaudRate);
            Channel.DtrEnable = true;
            Channel.Open();
            SendCom("sw resume: x=" + backupXYZ[0] + ",y=" + backupXYZ[1] + ",z=" + backupXYZ[2]);
            var resp = waitForResponse("sw resume", 1800);
        }
    }
    public class MarlinESSMachine: ESSMachine
    {
        float[] currentPositions = new float[3];
        float[] axisStepsPerUnitMarlin;
        float[] limits;
        float[] maxFeedRatesMarlin;
        float currentProgressMarlin = 0;
        string currentXyStatusMarlin = "Idle"; // Idle, Homing X, Homing Y, Coating, Moving, paused
        string currentPumpStatusMarlin = "Idle"; // Idle, paused, Pumping, Moving, Homing Z
        private string currentTemp = "--";
        float timeToPumpOrCoat = 0;
        DateTime systemStartAt = DateTime.Now;
        bool stopCoatFlag = false;
        bool pauseCoatFlag = false;
        bool tempAcquireEnabled = false;
        public MarlinESSMachine(SerialPort sp) : base(sp)
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);
                MarlinStatusWrapperSendStatus();
                while (sp.IsOpen)
                {
                    Thread.Sleep(1000);
                    if (tempAcquireEnabled)
                    {
                        if (!MarlinCommunication.InGetResponse) // we can skip getting a temp resp if there is other stuff that needs to be done
                        {
                            var resp = MarlinCommunication.GetResponse(Channel, "M105", 100); // get temp
                            if (resp.Length > 1)
                            {
                                currentTemp = resp[0].Split(new string[] { "T:" }, StringSplitOptions.RemoveEmptyEntries)[0].Split('/')[0];
                                MarlinStatusWrapperSendStatus();
                            }
                        }
                    }
                }
            }).Start();
        }
        long millis()
        { 
            return (long)((DateTime.Now - systemStartAt).TotalMilliseconds);
        }
        void UpdateXYPositionMarlin()
        {
            if (axisStepsPerUnitMarlin == null)
            {
                var m92 = MarlinCommunication.GetResponse(Channel, "M92");
                var line = m92[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                axisStepsPerUnitMarlin = new float[3];
                axisStepsPerUnitMarlin[0] = float.Parse(line[0].Substring(1));
                axisStepsPerUnitMarlin[1] = float.Parse(line[1].Substring(1));
                axisStepsPerUnitMarlin[2] = float.Parse(line[3].Substring(1));
            }
            if (maxFeedRatesMarlin == null)
            {
                var m203 = MarlinCommunication.GetResponse(Channel, "M203");
                var line = m203[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                maxFeedRatesMarlin = new float[3];
                maxFeedRatesMarlin[0] = float.Parse(line[0].Substring(1));
                maxFeedRatesMarlin[1] = float.Parse(line[1].Substring(1));
                maxFeedRatesMarlin[2] = float.Parse(line[3].Substring(1));

            }
            //foreach (var statusLine in MarlinCommunication.Flushed)
            //{
            //    if (statusLine.Contains("M203"))
            //    {
            //        var line = statusLine.Split(new string[] { "M203" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //        maxFeedRatesMarlin = new float[3];
            //        maxFeedRatesMarlin[0] = float.Parse(line[0].Substring(1));
            //        maxFeedRatesMarlin[1] = float.Parse(line[1].Substring(1));
            //        maxFeedRatesMarlin[2] = float.Parse(line[3].Substring(1));
            //    }
            //}
            var m211 = MarlinCommunication.GetResponse(Channel, "M211");
            if (m211.Length > 0)
            {
                foreach (var line in m211)
                {
                    if (line.Contains("Max:"))
                    {
                        var ff = line.Split(new string[] { "Max:" }, StringSplitOptions.RemoveEmptyEntries);
                        var pairs2 = line.Split(new string[] { "Max:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        limits = new float[3];
                        for (int i = 0; i < 3; i++)
                            limits[i] = float.Parse(pairs2[i].Substring(1));
                    }
                }
            }
            var pos = MarlinCommunication.GetResponse(Channel, "M114");
            if (pos.Length != 2)
                return;
            var pairs = pos[0].Split(' ');
            foreach (var pair in pairs)
            {
                try
                {
                    var key = pair.Split(':')[0].ToLower();
                    var value = pair.Split(':')[1];
                    var index = -1;

                    if (key[0] == 'x')
                        index = 0;
                    else if (key[0] == 'y')
                        index = 1;
                    else if (key[0] == 'e')
                        index = 2;
                    if (index >= 0)
                        currentPositions[index] = float.Parse(value) / axisStepsPerUnitMarlin[index];
                }
                catch { }
            }
            if (currentXyStatusMarlin != "Coating")
                currentXyStatusMarlin = "Moving";
            MarlinStatusWrapperSendStatus();
            if (currentXyStatusMarlin != "Coating")
            {
                currentXyStatusMarlin = "Idle";
                MarlinStatusWrapperSendStatus();
            }
        }
        List<string> respMarlin = new List<string>();
        void MarlinStatusWrapperSendStatus()
        {
            var args = new Dictionary<string, string>();
            args["x"] =
            currentPositions[0].ToString();
            args["y"] =
            currentPositions[1].ToString();
            args["z"] =
            currentPositions[2].ToString();
            args["progress"] =
            Math.Min(100, currentProgressMarlin).ToString();
            args["xy stage"] =
            currentXyStatusMarlin;
            args["pump"] =
            currentPumpStatusMarlin;
            args["temp"] =
            currentTemp;
            applyStatusArgs("status", args);
        }
        float coatX = 0, coatY = 0, coatWidth = 0, coatHeight = 0, pumpMax = 0, Q = 0, coatSpeed = 0, coatStepY = 0;
        int timesToCoat = 0;
        float pumpStart = 0;
        float totalLengthToCoat = 0;
        string F(string s)
        {
            return s;
        }
        string getCommand(string CommandRaw)
        {
            if (CommandRaw.IndexOf(F(":")) >= 0)
                return CommandRaw.Substring(0, CommandRaw.IndexOf(F(":"))).Trim();
            return CommandRaw;
        }
        Dictionary<string, string> getArgs(string CommandRaw)
        {
            if (CommandRaw.IndexOf(F(":")) < 0) // no args
                return new Dictionary<string, string>();
            else
            {
                return CommandRaw.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(pair => pair.Split('='))
                    .ToDictionary(val => val[0], val => val[1]);
            }
        }
        bool simulateOnly = false;
        float totalTimeInG1s = 0;
        float totalTravelInG1s = 0;

        void G1Blocking(G1Move move)
        {
            var dist = Math.Sqrt(move.X * move.X + move.Y * move.Y);
            var timeMin = dist / move.Feed;
            var timeMs = timeMin * 60 * 1000;
                totalTimeInG1s += (float)timeMs;
                totalTravelInG1s += (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (!simulateOnly)
            {
                if (timeMs < 200)
                    timeMs = 200;
                MarlinCommunication.GetResponse(Channel, $"G1 X{move.X} Y{move.Y} E{move.E} F{move.Feed}", 1000);
                var resp = MarlinCommunication.GetResponse(Channel, "M400", (int)(timeMs * 1.4));
                UpdateXYPositionMarlin();
            }
        }
        public override void SendCom(string command)
        {
            var Args = getArgs(command);
            command = getCommand(command);
            if (
                command.StartsWith("x+") || command.StartsWith("x-") ||
                command.StartsWith("y+") || command.StartsWith("y-") ||
                command.StartsWith("z+") || command.StartsWith("z-")
                )
            {
                var dict = new Dictionary<string, string>();
                string[] keys = { "y+", "y++", "y-", "y--", "x-", "x--", "x+", "x++", "z+", "z++", "z-", "z--", };
                var values = new string[] {
                    "G1 Y1 F2400", //"y+", 
                    "G1 Y10 F2400", //"y++", 
                    "G1 Y-1 F2400", //"y-",
                    "G1 Y-10 F2400", //"y--", 
                    "G1 X-1 F2400", //"x-", 
                    "G1 X-10 F2400", //"x--", 
                    "G1 X1 F2400", //"x+", 
                    "G1 X10 F2400", //"x++"
                    "G1 E-1 F2400", //"z+", 
                    "G1 E-10 F2400", //"z++", 
                    "G1 E1 F2400", //"z-",
                    "G1 E10 F2400", //"z--", 
                };
                for(int i =0;  i < keys.Length; i++)
                    dict[keys[i]]= values[i];

                MarlinCommunication.GetResponse(Channel, "G91"); // relative coordinates
                MarlinCommunication.GetResponse(Channel, "M83"); // relative pump coordinates

                if (command.Contains("z"))
                    currentPumpStatusMarlin = "Moving";
                else
                    currentXyStatusMarlin = "Moving";
                MarlinStatusWrapperSendStatus();
                MarlinCommunication.GetResponse(Channel, dict[command]);
                MarlinCommunication.GetResponse(Channel, "M400", 2000);
                if (command.Contains("z"))
                    currentPumpStatusMarlin = "Idle";
                else
                    currentXyStatusMarlin = "Idle";
                MarlinStatusWrapperSendStatus();
                UpdateXYPositionMarlin();
            }
            else if (command.StartsWith("heat"))
            {
                tempAcquireEnabled = true;
                var t = MarlinCommunication.GetResponse(Channel, "M104 S" + command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], 10000);
            }
            else if (command.StartsWith("cylinder"))
            {
                MarlinCommunication.GetResponse(Channel, "M18 Z", 100); // Stop Z first
                MarlinCommunication.GetResponse(Channel, "G1 Z" + command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], 10000);
            }
            else if (command.StartsWith("home xy"))
            {
                currentXyStatusMarlin = "Homing X";
                MarlinStatusWrapperSendStatus();
                MarlinCommunication.GetResponse(Channel, "G28 X0", 10000);

                currentXyStatusMarlin = "Homing Y";
                MarlinStatusWrapperSendStatus();
                MarlinCommunication.GetResponse(Channel, "G28 Y0", 30000);

                currentXyStatusMarlin = "Idle";
                MarlinStatusWrapperSendStatus();
                UpdateXYPositionMarlin();
            }
            else if (command.StartsWith("pause coat"))
            {
                pauseCoatFlag = true;
                if (MarlinCommunication.InGetResponse)
                {
                    MessageBox.Show("The process will pause after the current movement command", "Cannot stop instantaneously", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    while (MarlinCommunication.InGetResponse)
                        Thread.Sleep(30);
                }
            }
            else if (command.StartsWith("resume coat"))
            {
                pauseCoatFlag = false;
            }
            else if (command.StartsWith("abort"))
            {
                stopCoatFlag = true;
                if (MarlinCommunication.InGetResponse)
                {
                    MessageBox.Show("The process will abort after the current movement command", "Cannot stop instantaneously", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    while (MarlinCommunication.InGetResponse)
                        Thread.Sleep(30);
                }
                respMarlin.Add(F("abort resp: answer = stopped"));
            }
            else if (command.StartsWith("set coat"))
            {
                //respMarlin.Add(F("Set Coat")); delay(1);
                // this command must be used safely							 
                //int mode = Args[F("mode")).toInt();
                float startX = currentPositions[0];
                float startY = currentPositions[1];
                float x = startX, y = startY;
                float lenX = float.Parse(Args[F("lenX")]);   // mm
                float lenY = float.Parse(Args[F("lenY")]);   // mm
                coatStepY = float.Parse(Args[F("stepY")]); // ~ 
                timesToCoat = int.Parse(Args[F("coats")]);   // ~
                coatSpeed = float.Parse(Args[F("speed")]); // mm/sec
                Q = float.Parse(Args[F("Q")]) / 1000;        // mm/ms

                if (maxFeedRatesMarlin[2] < Q * 1000 || maxFeedRatesMarlin[1] < coatSpeed)
                {
                    respMarlin.Add(F("coat resp: answer = no, message = The set pumping speed is too much for the syringe pump."));
                    return;
                }
                if (int.Parse(Args[F("rstr")]) == 1)
                {
                    if (maxFeedRatesMarlin[0] < coatSpeed || maxFeedRatesMarlin[1] < coatSpeed)
                    {
                        respMarlin.Add(F("coat resp: answer = no, message = The set coat speed is too much for the linear stage."));
                        return;
                    }

                    if (currentPositions[0] + lenX > limits[0])
                    {
                        respMarlin.Add(F("coat resp: answer = no, message = The coat area width is outside the limits of the linear stage."));
                        return;
                    }
                    if (currentPositions[1] + lenY > limits[1])
                    {
                        respMarlin.Add(F("coat resp: answer = no, message = The coat area height is outside the limits of the linear stage."));
                        return;
                    }
                    if (coatSpeed <= 0 || coatSpeed > 25)
                    {
                        respMarlin.Add(F("coat resp: answer = no, message = The travel speed you entered is not within the possible hardware range, (0, 25] mm/s"));
                        return;
                    }
                }
                float maxDist = float.Parse(Args[F("mxd")]);
                if (maxDist > 0)
                {
                    if (limits[2] - maxDist < 0)
                    {
                        respMarlin.Add(F("coat resp: answer = no, message = The pump won't be able to push this amount of volume."));
                        return;
                    }
                }
                if (maxDist < 0) // not needed
                    pumpMax = 0;
                else
                    pumpMax = maxDist;
                pumpStart = currentPositions[2];

                float timeToPump = float.Parse(Args[F("mxt")]); // seconds
                if (timeToPump < 0)
                {
                    timeToPump = limits[2] / (Q * 1000);
                    this.timeToPumpOrCoat = timeToPump;
                    if (pumpMax > 0)
                    {
                        this.timeToPumpOrCoat = pumpMax / (Q * 1000);
                    }
                }
                else
                {
                    // to implement a more accurate time limit, lets use millis instead of setting a maxDist
                    this.timeToPumpOrCoat = timeToPump;
                }

                coatX = currentPositions[0];
                coatY = currentPositions[1];
                coatWidth = lenX;
                coatHeight = lenY;
                pauseCoatFlag = false;
                stopCoatFlag = false;
                currentProgressMarlin = 0;


                if (int.Parse(Args[F("rstr")]) == 0)
                {
                    coatWidth = 0;
                    coatHeight = 0;
                }
                else
                {
                    this.totalTimeInG1s = 0;
                    this.totalTravelInG1s = 0;
                    this.simulateOnly = true;
                    DoRaster();
                    this.simulateOnly = false;
                    totalLengthToCoat = this.totalTravelInG1s;
                }


                respMarlin.Add("coat resp: answer = yes, total length = " + totalLengthToCoat);
                MarlinCommunication.GetResponse(Channel, "M83");    
                if (int.Parse(Args[F("rstr")]) == 1)
                {
                    new Thread(() =>
                    {
                        this.totalTimeInG1s = 0;
                        this.totalTravelInG1s = 0;
                        simulateOnly = false;
                        DoRaster();
                    }).Start();
                }
                else
                {
                    // pump only
                    new Thread(() =>
                    {
                        currentPumpStatusMarlin = "Pumping";
                        double timeDone = 0;
                        MarlinCommunication.GetResponse(Channel, $"G1 F{1}");
                        while (true)
                        {
                            var st = DateTime.Now;
                            var dt = timeToPumpOrCoat - timeDone;
                            if (dt > 10)
                                dt = 10;
                            if (dt <= 0)
                                break;
                            float move = -0.1F;
                            float rate = 0.1F;
                            MarlinCommunication.GetResponse(Channel, $"G1 E{move} F{rate}");
                            MarlinCommunication.GetResponse(Channel, "M400", (int)(dt * 1200000));
                            var correct = Math.Abs(move / rate * 60);
                            var took = DateTime.Now - st;
                            var error = took.TotalSeconds / correct;
                            var faster = 1 / (error);
                            var resp = MarlinCommunication.GetResponse(Channel, "M205");
                            timeDone += dt;

                            while (pauseCoatFlag) Thread.Sleep(30);
                        }
                        currentPumpStatusMarlin = "Idle";
                    }).Start();
                }
            }
        }
        public void DoRaster()
        {
            float eMovePerWidth = (Q * 1000/*original is in mm/ms*/) * (coatWidth / coatSpeed) /*t*/;
            float eMovePerStep = (Q * 1000/*original is in mm/ms*/) * (coatStepY / coatSpeed) /*t*/;
            float currentTime = 0;

            if (!simulateOnly)
            {
                currentXyStatusMarlin = "Coating";
                currentPumpStatusMarlin = "Pumping";
                MarlinCommunication.GetResponse(Channel, "G91");
                MarlinStatusWrapperSendStatus();
                UpdateXYPositionMarlin();
            }
            if (timeToPumpOrCoat == -1)
                timeToPumpOrCoat = long.MaxValue;
            for (int ci = 0; ci < timesToCoat 
                && currentTime <= timeToPumpOrCoat
                && !stopCoatFlag; ci++)
            {
                float currentY = 0;
                float currentX = 0;
                while (currentY <= coatHeight && currentTime + coatWidth / coatSpeed <= timeToPumpOrCoat && !stopCoatFlag)
                {
                    while (pauseCoatFlag) Thread.Sleep(30);
                    // move left
                    G1Blocking(new G1Move(coatWidth, 0, eMovePerWidth, coatSpeed * 60 /*Feed is in mm/min*/));
                    currentProgressMarlin = totalTravelInG1s / totalLengthToCoat * 100;
                    currentX += coatWidth;
                    currentTime +=  coatWidth / coatSpeed;

                    while (pauseCoatFlag) Thread.Sleep(30);
                    // can we go up?
                    if (currentY + coatStepY <= coatHeight && currentTime + (coatWidth + coatStepY) / coatSpeed <= timeToPumpOrCoat && !stopCoatFlag) // safe to move up
                    {
                        // move Up and left
                        G1Blocking(new G1Move(0, coatStepY, eMovePerStep, coatSpeed * 60 /*Feed is in mm/min*/));
                        currentProgressMarlin = totalTravelInG1s / totalLengthToCoat * 100;
                        G1Blocking(new G1Move(-coatWidth, 0, eMovePerWidth, coatSpeed * 60 /*Feed is in mm/min*/));
                        currentProgressMarlin = totalTravelInG1s / totalLengthToCoat * 100;
                        currentX -= coatWidth;
                        currentY += coatStepY;
                        currentTime += coatWidth / coatSpeed;
                        currentTime += coatStepY / coatSpeed;
                    }
                    else
                    { break; }
                    // can we go up?
                    if (currentY + coatStepY <= coatHeight && currentTime + coatStepY / coatSpeed <= timeToPumpOrCoat && !stopCoatFlag) // safe to move up
                    {
                        // move Up 
                        G1Blocking(new G1Move(0, coatStepY, eMovePerStep, coatSpeed * 60 /*Feed is in mm/min*/));
                        currentProgressMarlin = totalTravelInG1s / totalLengthToCoat * 100;
                        currentY += coatStepY;
                        currentTime += coatStepY / coatSpeed;
                    }
                    else
                    { break; }
                }
                // we are at the top edge. Need to go to the left
                if (currentX > 0)
                {
                    // move Left

                    G1Blocking(new G1Move(-currentX, 0, 0, maxFeedRatesMarlin[0] * 60));
                    currentX = 0;
                }
                // we are at the top left. Need to go down
                if (currentY > 0)
                {
                    // move Down
                    G1Blocking(new G1Move(0, -currentY, 0, maxFeedRatesMarlin[1] * 60));
                    currentX = 0;
                }
            }
            if (!simulateOnly)
            {
                currentXyStatusMarlin = "Idle";
                currentPumpStatusMarlin = "Idle";
                MarlinStatusWrapperSendStatus();
                UpdateXYPositionMarlin();
                if (stopCoatFlag)
                    respMarlin.Add(F("abort resp: answer = stopped"));
                else
                    applyStatusArgs(F("coat end"));
                stopCoatFlag = false;
                pauseCoatFlag = false;
            }
        }
        public override string waitForResponse(string com, int timeOut)
        {
            var st = DateTime.Now;
            while((DateTime.Now - st).TotalMilliseconds < timeOut)
            {
                if (respMarlin.Count > 0)
                {
                    var resp = respMarlin.Last();
                    respMarlin.RemoveAt(respMarlin.Count - 1);
                    if (resp.StartsWith(com))
                        return resp;
                }
            }
            return "";
        }
        public override void InPlaceReset(float[] backupXYZ)
        {
            // do nothing
        }
        public override void Poller_Tick(object sender, EventArgs e)
        {
            if (inTick)
                return;
            inTick = true;
            if (Channel == null)
                return;
            if (!Channel.IsOpen)
                inTick = false;
        }
    }
    struct G1Move
    {
        public G1Move(float X, float Y, float E, float feed) : this()
        {
            this.X = X;
            this.Y = Y;
            this.E = E;
            this.Feed = feed;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float E { get; set; }
        public float Feed { get; set; }
    }
}