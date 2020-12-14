using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Sentinel.Domain;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Test
{
    public class MockHttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpResponseMessage _responseToSend;

        public MockHttpClientWrapper(HttpResponseMessage responseToSend)
        {
            _responseToSend = responseToSend;
        }

        public void Dispose()
        {
        }

        public void RegisterClient(ProxyInfo proxyInfo = null, bool followRedirects = true)
        {
        }

        public async Task<HttpResponseMessage> TrySendRequest(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            return _responseToSend;
        }
    }
}
