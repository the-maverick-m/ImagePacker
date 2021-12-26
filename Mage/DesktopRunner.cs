using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;

namespace Mage
{
    public class DesktopRunner
    {
        public static void Run<T>() where T : Game, new()
        {
#if DEBUG
            using (var game = new T())
                game.Run();
#else
            try
            {
                using (var game = new T())
                    game.Run();
            }
            catch (Exception e)
            {
                string oldText;
                using (StreamReader reader = new StreamReader(SaveData.ErrorLogPath))
                    oldText = reader.ReadToEnd();

                using (FileStream stream = File.OpenWrite(SaveData.ErrorLogPath))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine($"{nameof(T)} error log");
                    writer.WriteLine(DateTime.Now);
                    writer.WriteLine(e.Message);
                    writer.WriteLine(e.StackTrace);
                    writer.WriteLine("");
                    writer.WriteLine(oldText);
                }

                try
                {
                    Process.Start(SaveData.ErrorLogPath);
                }
                catch { }
            }
#endif
        }
    }
}