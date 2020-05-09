using System.Collections.Generic;
using Sentinel.Domain.Models.Authentication;

namespace Sentinel.Domain.Models.Scan
{
    public class Target
    {
        public string Url { get; set; }
        public bool UriIsRelative { get; set; }
        public bool UseHttps { get; set; }
        public ScanRules Rules { get; set; }
        public IAuthenticationDetail Authentication { get; set; }
        public Dictionary<string, string> CustomHeaders { get; set; }
    }
}