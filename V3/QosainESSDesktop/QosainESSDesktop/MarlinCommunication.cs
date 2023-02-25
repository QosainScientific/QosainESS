using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QosainESSDesktop
{
    public class MarlinCommunication
    {
        public static List<string> Flushed { get; private set; } = new List<string>();
        public static string TemperatureUpdate = "";
        public static bool InGetResponse { get; private set; }
        public static List<KeyValuePair<string, string>> Responses { get; private set; } = new List<KeyValuePair<string, string>>();
        public static string[] GetResponse(SerialPortStream sp, string com, int timeOut = 1000)
        {
            int flushedAtStart = Flushed.Count;
            InGetResponse = true;
            var st = DateTime.Now;
            sp.Encoding = new UTF8Encoding();
            sp.NewLine = "\n";
            sp.ReadTimeout = 100;
            if (!sp.IsOpen)
                return new string[0];
            while (sp.BytesToRead > 0)
            {
                try
                {
                    Flushed.Add(sp.ReadLine());
                    if (Flushed.Last().ToLower().Contains("error"))
                    {
                        var error = Flushed.Last();
                        ESSMachine.ThrowMachineError(error);
                    }
                    else if (Flushed.Last().Trim().StartsWith("T:")) // temp update
                    {
                        TemperatureUpdate = Flushed.Last();
                        Flushed.Remove(Flushed.Last());
                    }
                }
                catch
                {
                    try
                    {
                        Flushed.Add(sp.ReadExisting());
                    }
                    catch { }
                }
            }
            sp.ReadTimeout = timeOut;
            if (string.IsNullOrEmpty(com)) // empty com will just flush everything
            {
                InGetResponse = false;
                return new string[0];
            }
            sp.WriteLine(com);
            var resp = new List<string>();
            while (true)
            {
                try
                {
                    var str = sp.ReadLine().Trim();
                    if (str.StartsWith("T:")) // temp update
                        TemperatureUpdate = str;
                    else if (str == "ok")
                    {
                        resp.Add("ok");
                        break;
                    }
                    else if (str.StartsWith("ok")) // but str != "ok"
                    {
                        resp.Add(str.Substring(2).Trim());
                        resp.Add("ok");
                        break;
                    }
                    else if (str == "echo:busy: processing")
                        Flushed.Add(str);
                    else
                        resp.Add(str);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Flushed.AddRange(resp);
                    InGetResponse = false;
                    Responses.Add(new KeyValuePair<string, string>(com, "NO_RESPONSE"));
                    return new string[0];
                }
                catch (TimeoutException)
                {
                    Flushed.AddRange(resp);
                    InGetResponse = false;
                    Responses.Add(new KeyValuePair<string, string>(com, "NO_RESPONSE"));
                    if (com == "M114")
                        ;
                    return new string[0];
                }
            }
            InGetResponse = false;
            Responses.Add(new KeyValuePair<string, string>(com, string.Join(";", resp)));
            return resp.ToArray();
        }
    }
}
