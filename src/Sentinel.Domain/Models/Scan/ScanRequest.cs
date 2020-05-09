using System.Collections.Generic;

namespace Sentinel.Domain.Models.Scan
{
    public class ScanRequest
    {
        public string HostName { get; set; }
        public List<Target> Targets { get; set; }
        public bool AllowTargetsWithDifferentHostName { get; set; }
    }
}
