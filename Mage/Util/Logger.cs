using System;

namespace Mage
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public static class Logger
    {
        public static void Log(string message, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                case LogLevel.Warning:
                    Console.WriteLine(message);
                    break;

                case LogLevel.Error:
                    Console.Error.WriteLine(message);
                    break;
            }
        }
    }
}