using System;

namespace Etirps.RiZhi.Models
{
    public class Entry
    {
        public DateTime Timestamp { get; set; }

        public string CallingFile { get; set; }

        public string CallingMethod { get; set; }

        public int CallingLine { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }
    }
}
