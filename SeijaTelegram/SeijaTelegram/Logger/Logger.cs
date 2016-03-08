using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeijaTelegram
{
    class Logger
    {
        public static void Log(string message)
        {
            TimeLog(message);
        }

        public static void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            TimeLog(message);
            Console.ResetColor();
        }

        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            TimeLog(message);
            Console.ResetColor();
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            TimeLog(message);
            Console.ResetColor();
        }

        public static void Error(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            TimeLog(String.Format("Exception: {0}", ex.Message));
            Console.ResetColor();
        }

        static void TimeLog(string message)
        {
            Console.WriteLine("[{0}] {1}", DateTime.Now, message);
        }
    }
}
