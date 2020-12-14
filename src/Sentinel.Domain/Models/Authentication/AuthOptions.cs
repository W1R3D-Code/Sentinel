using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sentinel.Domain.Models.Authentication;

namespace Sentinel.Domain.Models.Scan
{
    public class AuthOptions
    {
        public AuthenticationType Type { get; set; }
        public Dictionary<string, string> Options { get; set; }

        [JsonConstructor]
        public AuthOptions(AuthenticationType type, Dictionary<string, string> options)
        {
            Type = type;
            Options = options ?? new Dictionary<string, string>();
        }

        public IAuthenticationDetail ToAuthenticationDetail(string host = null)
        {
            try
            {
                switch (Type)
                {
                    case AuthenticationType.Forms:
                        return ToFormsAuth(host);
                    case AuthenticationType.Basic:
                        return ToBasicAuth();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private IAuthenticationDetail ToBasicAuth()
        {
            if (!Options.ContainsKey("username") || Options["username"] == null)
                throw new ArgumentNullException("username");

            if (!Options.ContainsKey("password") || Options["password"] == null)
                throw new ArgumentNullException("password");

            var basicAuth = new BasicAuthentication(Options["username"], Options["password"]);
            return basicAuth;
        }

        private FormBasedAuthentication ToFormsAuth(string host)
        {
            Uri url;

            if (!Options.ContainsKey("url") || Options["url"] == null)
                throw new ArgumentNullException("url");
            else
                url = GetUrl("url", host);

            if (!Options.ContainsKey("username") || Options["username"] == null)
                throw new ArgumentNullException("username");

            if (!Options.ContainsKey("password") || Options["password"] == null)
                throw new ArgumentNullException("password");

            var formsAuth = new FormBasedAuthentication(url, Options["username"], Options["password"]);

            if (Options.ContainsKey("antiCsrfUri") && Options["antiCsrfUri"] != null)
                formsAuth.AntiCsrfUri = GetUrl("antiCsrfUri", host);

            if (Options.ContainsKey("antiCsrfTokenName") && Options["antiCsrfTokenName"] != null)
                formsAuth.AntiCsrfTokenName = Options["antiCsrfTokenName"];

            if (Options.ContainsKey("requestEncoding") && Options["requestEncoding"] != null && Enum.TryParse<RequestEncoding>(Options["requestEncoding"], out var requestEncoding))
                formsAuth.RequestEncoding = requestEncoding;

            return formsAuth;
        }

        private Uri GetUrl(string key, string host)
        {
            Uri uri = null;

            try {
                uri = new Uri(Options[key]);
            }
            catch (UriFormatException e) {
                // ignore
            }

            if (string.IsNullOrWhiteSpace(uri?.Host) && !string.IsNullOrWhiteSpace(host))
                uri = new Uri($"https://{host}{Options[key]}");

            return uri;
        }
    }
}