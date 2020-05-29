using System;

namespace Etirps.RiZhi.ExampleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var log = new RiZhi
            {
                FilePrefix = "ExampleApp",
                AssemblyVersion = "1.0.0",
                AssemblyName = "Example Program"
            };

            log.Debug("Debug message, appears first");
            log.Information("Information message, appears second");
            log.Warning($"Warning message, appears third. Error Flag = {log.ErrorFlag}");
            log.Error("Error message, appears fourth");

            if (log.ErrorFlag)
            {
                log.Debug($"Error was logged");
            }

            log.WriteLog();
            
        }
    }
}
