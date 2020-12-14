using System.Collections.Generic;

namespace Sentinel.Domain
{
    public static class Tokens
    {
        public static readonly List<string> KnownSessionTokens = new List<string>
        {
            "asp.net_sessionid",
            "aspsessionid",
            "siteserver",
            "cfid",
            "cftoken",
            "jsessionid",
            "phpsessid",
            "sessid",
            "sid",
            "viewstate",
            "zenid"
        };

        public static readonly List<string> KnownAntiCsrfTokens = new List<string>
        {
            "anticsrf",
            "CSRFToken",
            "__RequestVerificationToken",
            "csrfmiddlewaretoken",
            "authenticity_token",
            "OWASP_CSRFTOKEN",
            "anoncsrf",
            "csrf_token",
            "_csrf",
            "_csrfSecret",
            "__csrf_magic"
        };

        public static readonly List<string> KnownFalsePositiveTokens = new List<string>
        {
            "ARRAffinity", // Azure load balancer session
            "cookiesession1", // Fortinet WAF Session
        };
    }
}
