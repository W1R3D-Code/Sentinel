using System.Collections.Generic;
using System.Net.Http;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Scanner
{
    public interface IPassiveScanner
    {
        string Name();
        string Details();
        Severity DefaultSeverity();
        ScanResult Run(HttpResponseMessage response);
        bool IsEnabled();
    }
}
