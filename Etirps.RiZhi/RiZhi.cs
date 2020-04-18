using Etirps.RiZhi.Models;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Etirps.RiZhi
{
    public class RiZhi
    {
        private readonly ConcurrentQueue<Entry> _entryList;

        public bool ErrorFlag { get; private set; } = false;
        public string OutputDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        public string FilePrefix { get; set; } = "ScribeLog";


        public RiZhi()
        {
            _entryList = new ConcurrentQueue<Entry>();
        }

        public void WriteLog()
        {
            string outputFileName = Path.Combine(OutputDirectory, $"{FilePrefix}_{ DateTime.Now.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture)}.log");
            string logOutput = "";

            foreach (var entry in _entryList)
            {
                if (entry == null) { continue; }
                logOutput += $"{entry.Level} | {entry.Timestamp} | {entry.CallingFile}->{entry.CallingMethod}->{entry.CallingLine} | {entry.Message}\n";
            }

            File.WriteAllText(outputFileName, logOutput);
        }

        public void Debug(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string sourceMethod = "",
            [CallerLineNumber] int sourceLine = 0)
        {
            AddEntry(message, LogLevel.DEBUG, sourceFile, sourceMethod, sourceLine);
        }

        public void Information(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string sourceMethod = "",
            [CallerLineNumber] int sourceLine = 0)
        {
            AddEntry(message, LogLevel.INFO, sourceFile, sourceMethod, sourceLine);
        }

        public void Warning(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string sourceMethod = "",
            [CallerLineNumber] int sourceLine = 0)
        {
            AddEntry(message, LogLevel.WARN, sourceFile, sourceMethod, sourceLine);
        }

        public void Error(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string sourceMethod = "",
            [CallerLineNumber] int sourceLine = 0)
        {
            // We record if an error occured, in case the user wants to know
            ErrorFlag = true;
            AddEntry(message, LogLevel.ERROR, sourceFile, sourceMethod, sourceLine);
        }

        private void AddEntry(string message, LogLevel level, string sourceFile, string sourceMethod, int sourceLine)
        {
            // Source file is the entire path, only the file name is relevant
            var fileName = Path.GetFileName(sourceFile);

            var newEntry = new Entry
            {
                Message = message,
                Level = level,
                CallingFile = fileName,
                CallingMethod = sourceMethod,
                CallingLine = sourceLine,
                Timestamp = DateTime.Now
            };

            _entryList.Enqueue(newEntry);
        }
    }
}
