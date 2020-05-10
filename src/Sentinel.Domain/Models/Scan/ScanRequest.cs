using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Sentinel.Domain.Models.Scan
{
    public class ScanRequest
    {
        public string Host { get; set; }
        public List<Target> Targets { get; set; }
        public bool AllowTargetsWithDifferentHostName { get; set; }

        public ScanRequest(Target target, string host = null, bool allowTargetsWithDifferentHost = false) :
            this(new List<Target> {target}, host, allowTargetsWithDifferentHost)
        {
        }

        [JsonConstructor]
        public ScanRequest(List<Target> targets, string host = null, bool allowTargetsWithDifferentHost = false) : this(host)
        {
            if (targets.Any(x => x.UriIsRelative) && string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host));

            Targets = targets;
            AllowTargetsWithDifferentHostName = allowTargetsWithDifferentHost;
        }

        public ScanRequest(string host) => Host = host;
    }
}
