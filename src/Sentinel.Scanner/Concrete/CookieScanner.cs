using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Sentinel.Domain;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Scanner
{
    public class CookieScanner : PassiveScannerBase
    {
        public CookieScanner(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public override string Name() => "Cookie Scanner";
        public override string Details() => "Scan cookies to check for possible attack vectors";
        public override Severity DefaultSeverity() => Severity.Low;

        

        private ScanMetaData MetaData => new ScanMetaData(Name(), Details(), DefaultSeverity());

        public override ScanResult Run(HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var result = new ScanResult(MetaData);

            var cookies = response.GetCookiesSetOnResponse();

            if (cookies == null || cookies.Count == 0)
            {
                result.Summary = "No Cookies Detected";
                return result;
            }

            foreach (Cookie cookie in cookies)
            {
                if (Tokens.KnownFalsePositiveTokens.Contains(cookie.Name))
                    continue;

                if (!cookie.Secure)
                    return result.WithSingleFailure("Cookie set without secure flag", GetSeverity(cookie.Name),
                        new[] {cookie.Name});
            }

            return result;
        }

        private Severity GetSeverity(string cookieName)
        {
            return Tokens.KnownAntiCsrfTokens.Contains(cookieName) || Tokens.KnownSessionTokens.Contains(cookieName)
                ? Severity.Medium
                : DefaultSeverity();
        }

        public override bool IsEnabled() => true; //TODO:: Get from config

    }
}