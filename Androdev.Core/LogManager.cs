using System;
using System.Diagnostics;
using System.IO;

namespace Androdev.Core
{
    public class LogManager
    {
        private const string LogFormatString = "{0}  {1} {2}: {3}";
        private const string ShortDateFormat = "yyyy-MM-dd";        // ISO8601
        private const string LongDateFormat = "yyyyMMdd'T'HHmmss";  // ISO8601

        private readonly string _logClass;

        public static LogManager GetClassLogger()
        {
            var tempTrace = new StackTrace();
            var tempMethod = tempTrace.GetFrame(1).GetMethod();

            if (tempMethod.ReflectedType == null)
            {
                return new LogManager("UNTRACEABLE");
            }
            else
            {
                return new LogManager(tempMethod.ReflectedType.Name);
            }
        }

        public static void ConfigureLogger()
        {
            // configure
            Trace.AutoFlush = true;
            Trace.Listeners.Clear();

            // create daily log
            var currentFilePath = DateTime.Now.ToString(ShortDateFormat) + ".txt";
            var fullPath = Path.Combine(Commons.GetBaseDirectoryPath(), currentFilePath);

            // add trace listener
            Trace.Listeners.Add(new TextWriterTraceListener(fullPath, "MainLogger"));
        }

        public LogManager(string className)
        {
            _logClass = "(" + className + ")";
        }

        public void Error(string message)
        {
            WriteEntry(message, "[ERROR] ");
        }

        public void Error(Exception ex)
        {
            WriteEntry("Exception occured.\r\n" + ex, "[ERROR] ");
        }

        public void Error(string message, Exception ex)
        {
            WriteEntry(message + "\r\n" + ex, "[ERROR] ");
        }

        public void Warning(string message)
        {
            WriteEntry(message, "[WARNING]");
        }

        public void Info(string message)
        {
            WriteEntry(message, "[INFO]  ");
        }

        public void Debug(string message)
        {
            WriteEntry(message, "[DEBUG] ");
        }

        private void WriteEntry(string message, string level)
        {
            Trace.WriteLine(string.Format(LogFormatString, DateTime.Now.ToString(LongDateFormat), level, _logClass, message));
        }
    }
}
