using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csmt
{
    class Log
    {
        static object o = new object();
        public static bool WriteLog = true;

        public static void Write(string s)
        {
            if (!WriteLog)
                return;

            lock (o)
            {
                try
                {
                    var p = Path.Combine(Path.Combine(Environment.CurrentDirectory, "log"),
                        DateTime.Now.ToString("dd-MM-yy HH_mm_ss_ffff") + ".log");
                    File.WriteAllText(p, s);

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(s);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                catch(Exception)
                { }
            }

        }
    }
}
