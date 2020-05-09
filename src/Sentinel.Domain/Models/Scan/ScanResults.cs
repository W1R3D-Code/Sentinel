using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sentinel.Domain.Extensions;

namespace Sentinel.Domain.Models.Scan
{
    public class ScanResults
    {
        public string HostName { get; set; }
        public List<ScanResult> Results { get; set; }
        public Dictionary<Severity, int> Summary => Results.ToSeveritySummary();

        public ScanResults(string hostName)
        {
            HostName = hostName;
            Results = new List<ScanResult>();
        }
    }
}