namespace ModTek.Util
{
    using System;
    using System.IO;

    internal static class Logger
    {
        private static StreamWriter logStream;

        internal static string LogPath { get; set; }

        internal static void CloseLogStream()
        {
            logStream?.Dispose();
            logStream = null;
        }

        internal static void FlushLogStream()
        {
            logStream?.Flush();
        }

        internal static void Log(string message)
        {
            var stream = GetOrCreateStream();
            stream?.WriteLine(message);
        }

        internal static void Log(string message, params object[] formatObjects)
        {
            var stream = GetOrCreateStream();
            stream?.WriteLine(message, formatObjects);
        }

        internal static void LogException(string message, Exception e)
        {
            var stream = GetOrCreateStream();
            if (stream == null)
            {
                return;
            }

            stream.WriteLine(message);
            stream.WriteLine(e.ToString());
            FlushLogStream();
        }

        internal static void LogWithDate(string message, params object[] formatObjects)
        {
            var stream = GetOrCreateStream();
            stream?.WriteLine(DateTime.Now.ToLongTimeString() + " - " + message, formatObjects);
        }

        private static StreamWriter GetOrCreateStream()
        {
            if (logStream == null && !string.IsNullOrEmpty(LogPath))
            {
                logStream = File.AppendText(LogPath);
            }

            return logStream;
        }
    }
}
