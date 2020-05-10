using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Sentinel.Domain;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Scanner
{
    public class ClickjackingScanner : PassiveScannerBase
    {
        public ClickjackingScanner(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public override string Name() => "ClickJacking Scanner";
        public override string Details() => "Scan response headers to check for vulnerability to clickjacking attacks " +
                                            $"by checking {HeaderNames.XFrameOptions} " +
                                            $"and {HeaderNames.ContentSecurityPolicy} (frame-ancestor directive) set on the response." +
                                            $"For more information on clickjacking, and ways you can mitigate it, see https://cheatsheetseries.owasp.org/cheatsheets/Clickjacking_Defense_Cheat_Sheet.html";
        public override Severity DefaultSeverity() => Severity.Low;

        private ScanMetaData MetaData => new ScanMetaData(Name(), Details(), DefaultSeverity());

        public override ScanResult Run(HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var result = new ScanResult(MetaData);

            var vulnerableCspDirectiveSources = new List<string>()
            {
                "*",
                "http:",
                "https:"
            };

            var xFrameExists = response.Headers.TryGetValues("X-Frame-Options", out var xFrameValues);
            var cspValues = response.Headers.GetCspDirectives();
            var cspExists = cspValues.Any(); 

            if (xFrameExists == false && cspExists == false)
                return result.WithSingleFailure("Neither X-Frame-Options or Content-Security-Policy are not set.");

            if (xFrameExists &&
                xFrameValues.Any(x => string.Equals(x, "allow-from *", StringComparison.OrdinalIgnoreCase)))
                return result.WithSingleFailure("X-Frame-Options is set but allows any domain");

            if (cspExists && cspValues.ContainsKey("frame-ancestors") && cspValues["frame-ancestors"]
                    .Any(x => vulnerableCspDirectiveSources.Contains(x.ToLower())))
            {
                var sources = cspValues["frame-ancestors"];
                result.WithSingleFailure($"Content-Security-Policy frame-ancestors directive allows any domain.{(sources.Contains("'self'") && sources.Contains("https:") ? " Possible misconfiguration detected; CSP Directive allow sources which match any of the specified sources, not all of them. frame-ancestors 'self' https:; will allow any source using HTTPS, not restrict sources to self WITH HTTPS." : string.Empty)}");
            }
            else if (cspExists)
                return result.WithSingleFailure("Content-Security-Policy does not include a frame-ancestors directive.");

            return result;
        }

        public override bool IsEnabled() => true; //TODO:: Get from config

    }
}