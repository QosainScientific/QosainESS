using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QosainESSDesktop
{
    public class MarlinCommunication
    {
        public static List<string> Flushed { get; private set; } = new List<string>();
        public static string[] GetResponse(SerialPort sp, string com, int timeOut = 1000)
        {
            var st = DateTime.Now;
            sp.Encoding = new UTF8Encoding();
            sp.NewLine = "\n";
            sp.ReadTimeout = 100;
            while (sp.BytesToRead > 0)
            {
                try
                {
                    Flushed.Add(sp.ReadLine());
                }
                catch
                {
                    Flushed.Add(sp.ReadExisting());
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
                    if (resp.Last() == "ok")
                        // already done!
                        break;
                }
                catch (TimeoutException)
                {
                    Flushed.AddRange(resp);
                    return new string[0];
                }

            }
            return resp.ToArray();
        }
    }
}
