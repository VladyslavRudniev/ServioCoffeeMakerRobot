using System;
using System.Diagnostics;
using System.IO;

namespace ServioCoffeMakerRobot
{
    public static class LoggerService
    {

        private static string logPath = "Log.txt";
        private static string dirName = "Log";

        public static async void Write(string prefix, string message)
        {
            try
            {
                var filename = "[" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "]" + logPath;
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dirName);
                
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (StreamWriter writer = new StreamWriter(Path.Combine(path, filename), true))
                {
                    await writer.WriteLineAsync($"[{DateTime.Now}] [{prefix}] {message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logger error: {ex.Message}");
            }
        }
    }
}
