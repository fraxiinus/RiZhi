# RiZhi 日誌
**A super simple log library**

![Build](https://img.shields.io/github/workflow/status/leeanchu/RiZhi/master?style=flat-square) ![NuGet](https://img.shields.io/nuget/v/Etirps.Rizhi?style=flat-square)

## Example

````C#
var log = new RiZhi();

log.Debug("Debug Message...");

log.Information("Info Message...");

log.Warning("Warning Message...");

log.Error("Error Message...");

// Error flag is true if an error was logged
if (log.ErrorFlag)
{
    log.WriteLog();
}

/* OUTPUT: ./logs/RiZhi_20200416_20:43.log
DEBUG | 2020-04-16 8:42:19 PM | Test.cs->Test->3 | Debug Message
*/

````