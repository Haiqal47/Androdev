//     This file is part of Androdev.
// 
//     Androdev is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Androdev is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with Androdev.  If not, see <http://www.gnu.org/licenses/>.
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
