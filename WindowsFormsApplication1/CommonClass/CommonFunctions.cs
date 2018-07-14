using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;



namespace WindowsFormsApplication1.CommonClass
{
    class CommonFunctions
    {

        public void Logger(String FunctionName, String Log)
        {

            string path = Application.StartupPath + "\\LogFile.txt";

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.WriteLine("Welcome To Log File...." + DateTime.Now);
                    tw.Close();
                }
            }
            if (File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(DateTime.Now + "-" + FunctionName + " - " + Log);
                tw.Close();
            }
        }


        public void LoggerTimerInterval(String FunctionName, String Log)
        {

            string path = Application.StartupPath + "\\TimerInterval.txt";

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.WriteLine("Welcome To Log File...." + DateTime.Now);
                    tw.Close();
                }
            }
            if (File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(DateTime.Now + "-" + FunctionName + " - " + Log);
                tw.Close();
            }
        }
    }
}
