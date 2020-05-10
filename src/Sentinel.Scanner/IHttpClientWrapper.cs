using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sentinel.Scanner
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> TrySendRequest(HttpRequestMessage message, CancellationToken cancellationToken, bool disposeClient = false);
    }
}