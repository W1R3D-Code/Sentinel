using System;
using System.Collections.Generic;
using System.Linq;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Domain.Extensions
{
    public static class ScanResultExtensions
    {
        public static Dictionary<Severity, int> ToSeveritySummary(this IDictionary<string, ScanResult> results)
        {
            var severities = Enum.GetValues(typeof(Severity)).Cast<Severity>();
            
            var summary = severities
                .ToDictionary(severity => severity, severity => results.Count(x => x.Value.Severity == severity));

            return summary;
        }
    }
}
