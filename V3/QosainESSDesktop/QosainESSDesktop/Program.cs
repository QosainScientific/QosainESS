using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QosainESSDesktop
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += (s, e) => HandleFatalException(e.Exception, "ErrorLogs", false);
            try
            {
                Application.Run(new MainForm());
                HandleFatalException(null, "Logs", true);
            }
            catch (Exception ex)
            {
                HandleFatalException(ex, "ErrorLogs", false);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleFatalException((Exception)e.ExceptionObject, "ErrorLogs", false);
        }
        static void HandleFatalException(Exception ex, string logType, bool discrete)
        {

            var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ESS", logType, DateTime.Now.ToString("yyMMddTHHmmss") + ".log");
            var message =
                DateTime.Now.ToString("yyyy MM dd THHmm:ss") + "\r\n" +
                (ex == null ? "" : (ex + "\r\n\r\n")) +
                "Coms: " + string.Join(";", MarlinESSMachine.Coms) + "\r\n\r\n" +
                "Responses:\r\n" + string.Join("\r\n", MarlinCommunication.Responses.Select(p => p.Key + "=" + p.Value)) + "\r\n\r\n---------------";
            if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                }
                catch
                {
                    MessageBox.Show("Could not create log directory: " + Path.GetDirectoryName(file));
                }
            }
            try
            {
                File.AppendAllText(file, message);
                if (!discrete)
                    MessageBox.Show("The application has crashed unexpectedly. Kindly see the error log at: " + file + "\r\n\r\n" + ex);
            }
            catch
            {
                MessageBox.Show("Could not write to the log file: " + file);
                MessageBox.Show("The application has crashed unexpectedly. The error returned is: \r\n" + message);
            }
        }
    }
}
