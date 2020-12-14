using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Domain
{
    public interface IHttpClientWrapper : IDisposable
    {
        void RegisterClient(ProxyInfo proxyInfo = null, bool followRedirects = true);
        Task<HttpResponseMessage> TrySendRequest(HttpRequestMessage message, CancellationToken cancellationToken);
    }
}