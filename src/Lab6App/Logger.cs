using System;
using System.IO;

namespace Lab6App
{
    public static class Logger
    {
        private static readonly object sync = new object();
        private static string? path;

        public static void Init(string filename = "logs/lab6.log")
        {
            try
            {
                var dir = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                path = filename;
                Log("Logger initialized.");
            }
            catch { }
        }

        public static void Log(string msg)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return;
                lock (sync)
                {
                    var line = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | {msg}";
                    File.AppendAllText(path, line + Environment.NewLine);
                }
            }
            catch { }
        }
    }
}
