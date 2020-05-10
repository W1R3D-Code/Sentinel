using System;
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

        public Target() => CustomHeaders = new Dictionary<string, string>();

        public static Target FromUri(string uri, bool useHttps = true) => new Target().WithUri(uri, useHttps);
        public static Target FromUri(Uri uri, bool useHttps = true) => new Target().WithUri(uri, useHttps);

        public Target WithUri(string url, bool useHttps = true)
        {
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
                throw new ArgumentException(nameof(url));

            return this.WithUri(uri, useHttps);
        }

        public Target WithUri(Uri uri, bool useHttps = true)
        {
            Url = uri.ToString();
            UriIsRelative = !uri.IsAbsoluteUri;
            UseHttps = useHttps;

            return this.WithDefaultSafeScanRules();
        }

        public Target WithDefaultSafeScanRules() => this.WithScanRules(null);

        public Target WithScanRules(ScanRules rules)
        {
            Rules = rules ?? ScanRules.DefaultSafeScanRules();
            return this;
        }

        public Target WithAuthentication(IAuthenticationDetail authentication)
        {
            Authentication = authentication;
            return this;
        }

        public Target WithCustomHeaders(Dictionary<string, string> customHeaders)
        {
            CustomHeaders = customHeaders ?? new Dictionary<string, string>();
            return this;
        }
    }
}