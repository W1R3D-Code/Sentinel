using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Domain.Extensions
{
    public static class ScanResultExtensions
    {
        public static Dictionary<Severity, int> ToSeveritySummary(this List<ScanResult> results)
        {
            var severities = Enum.GetValues(typeof(Severity)).Cast<Severity>();
            
            var summary = severities
                .ToDictionary(severity => severity, severity => results.Count(x => x.Severity == severity));

            return summary;
        }
    }
}
