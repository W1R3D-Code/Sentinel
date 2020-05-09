using System;
using System.Linq;
using System.Net.Http;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Domain.Extensions
{
    public static class TargetExtensions
    {
        public static HttpRequestMessage ToGetRequestMessage(this Target target, string host = null)
        {
            if (target.UriIsRelative && string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host));

            var scheme = target.UseHttps || target.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase)
                ? Uri.UriSchemeHttps
                : Uri.UriSchemeHttp;

            var uri = target.Url.Trim('/');

            if (target.UriIsRelative)
                uri = $"{scheme}{Uri.SchemeDelimiter}{host}/{uri}";
            else if (target.UseHttps &&
                uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                uri = $"{Uri.UriSchemeHttps}{uri.Substring(4)}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri)
            };

            if (target.CustomHeaders != null)
                foreach (var customHeader in target.CustomHeaders)
                    request.Headers.Add(customHeader.Key, customHeader.Value);

            if (target.Authentication != null)
            {
                target.Authentication.PreRequestAction();
                target.Authentication.AddRequestAuthentication(request);
            }

            return request;
        }
    }
}
