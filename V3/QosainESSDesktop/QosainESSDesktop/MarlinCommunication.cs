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
        public static bool InGetResponse { get; private set; }
        public static List<KeyValuePair<string, string>> Responses { get; private set; } = new List<KeyValuePair<string, string>>();
        public static string[] GetResponse(SerialPortStream sp, string com, int timeOut = 1000)
        {
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
            sp.WriteLine(com);
            var resp = new List<string>();
            while (true)
            {
                try
                {
                    resp.Add(sp.ReadLine());
                    var str = resp.Last().Trim();
                    if (str.StartsWith("ok") && str != "ok")
                    {
                        resp.RemoveAt(resp.Count - 1);
                        resp.Add(str.Substring(2).Trim());
                        resp.Add("ok");
                        break;
                    }
                    if (str == "ok")
                        // already done!
                        break;
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
                    return new string[0];
                }

            }
            InGetResponse = false;
            Responses.Add(new KeyValuePair<string, string>(com, string.Join(";", resp)));
            return resp.ToArray();
        }
    }
}
