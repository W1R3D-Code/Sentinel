using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sentinel.Domain;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Scanner.Helpers
{
    public class RequestHelper
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly ILoggerFactory _logger;

        public RequestHelper(IHttpClientWrapper httpClientWrapper, ILoggerFactory logger)
        {
            _httpClientWrapper = httpClientWrapper;
            _logger = logger;
        }

        public HttpRequestMessage ToGetRequestMessage(Target target, string host = null) => ToGetRequestMessageAsync(target, host).Result;

        public async Task<HttpRequestMessage> ToGetRequestMessageAsync(Target target, string host = null)
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
                await target.Authentication.PreRequestAction(_httpClientWrapper);
                target.Authentication.AddRequestCookies(request);
            }

            return request;
        }
    }
}
