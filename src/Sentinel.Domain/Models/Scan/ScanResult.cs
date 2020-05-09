using System.Collections.Generic;
using System.Linq;
using Sentinel.Domain.Extensions;

namespace Sentinel.Domain.Models.Scan
{
    public class ScanResult
    {
        public ScanMetaData Scan { get; }
        public bool Passed => !Failures.Any();
        public string Summary { get; set; }
        public List<ScanFailure> Failures { get; set; }
        public Severity? Severity { get; set; }
        public ScanResult(ScanMetaData metaData)
        {
            Scan = metaData;
            Failures = new List<ScanFailure>();
        }

        public void AddFailure(string message, Severity? severity = null, IEnumerable<string> failingItems = null)
        {
            Failures.Add(new ScanFailure(message, failingItems));

            if (!Severity.HasValue && !severity.HasValue)
                Severity = Scan.Severity;
            else if (severity.HasValue)
                Severity = Severity?.ElevateSeverityIfApplicable(severity.Value) ?? severity.Value;
        }

        public ScanResult WithSummary(string summary)
        {
            Summary = summary;
            return this;
        }

        public ScanResult WithSingleFailure(string message, Severity? severity = null, IEnumerable<string> failingItems = null)
        {
            AddFailure(message, severity, failingItems);
            return this.WithSummary(message);
        }
    }
}