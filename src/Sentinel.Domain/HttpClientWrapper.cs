using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Domain
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly ILogger _logger;
        private HttpClient _client;

        public HttpClientWrapper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(nameof(HttpClientWrapper));
            _client = null;
        }

        public void RegisterClient(ProxyInfo proxyInfo = null, bool followRedirects = true)
        {
            var clientHandler = proxyInfo != null
                ? new HttpClientHandler { Proxy = proxyInfo.ToWebProxy(), AllowAutoRedirect = followRedirects }
                : new HttpClientHandler { AllowAutoRedirect = followRedirects };

            _client?.Dispose();
            _client = new HttpClient(clientHandler);
        }

        public async Task<HttpResponseMessage> TrySendRequest(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            try
            {
                if (_client == null)
                    throw new Exception("No Client Registered");
                
                return await _client.SendAsync(message, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public void Dispose() => _client?.Dispose();
    }
}
