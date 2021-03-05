using Etirps.RiZhi.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Etirps.RiZhi
{
    public class RiZhi
    {
        private readonly ConcurrentQueue<Entry> _entryList;

        public bool ErrorFlag { get; private set; } = false;
        public string OutputDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        public string FilePrefix { get; set; } = "RiZhiLog";
        public string AssemblyName { get; set; }
        public string AssemblyVersion { get; set; }

        public RiZhi()
        {
            _entryList = new ConcurrentQueue<Entry>();

            var frames = new StackTrace().GetFrames();
            var initialAssembly = frames.Select(x => x.GetMethod().ReflectedType.Assembly).Distinct().Last().GetName();
            AssemblyVersion = initialAssembly.Version.ToString(2);
            AssemblyName = initialAssembly.Name;
        }

        public void WriteLog()
        {
            var outputFileName = Path.Combine(OutputDirectory, $"{FilePrefix}_{ DateTime.Now.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture)}.log");
            var logOutput = "";

            logOutput += $"Log file created by RiZhi for {AssemblyName} v{AssemblyVersion}\n";

            foreach (var entry in _entryList)
            {
                if (entry == null) { continue; }
                logOutput += ConstructLine(entry);
            }

            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }

            File.WriteAllText(outputFileName, logOutput);
        }

        private string ConstructLine(Entry entry)
        {
            var trace = $"{entry.CallingFile} -> {entry.CallingMethod}() -> {entry.CallingLine}";
            var level = string.Format("{0,-5} | {1,-22} | {2} | {3}", entry.Level, entry.Timestamp.ToString("s"), trace, entry.Message) + "\n";
            return level;
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
