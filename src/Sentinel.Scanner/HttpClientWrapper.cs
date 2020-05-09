using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sentinel.Scanner
{
    public class HttpClientWrapper : IRequester
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public HttpClientWrapper(ILoggerFactory logger, HttpClient client = null)
        {
            _logger = logger.CreateLogger(nameof(HttpClientWrapper));
            _client = client;
        }

        public async Task<HttpResponseMessage> TrySendRequest(HttpRequestMessage message, CancellationToken cancellationToken, bool disposeClient = false)
        {
            try
            {
                return await _client.SendAsync(message, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
            finally
            {
                if (disposeClient)
                    _client?.Dispose();
            }
        }
    }
}
