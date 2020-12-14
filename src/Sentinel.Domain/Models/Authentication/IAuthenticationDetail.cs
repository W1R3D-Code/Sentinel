using System.Net.Http;
using System.Threading.Tasks;

namespace Sentinel.Domain.Models.Authentication
{
    public interface IAuthenticationDetail
    {
        Task PreRequestAction(IHttpClientWrapper client);
        void AddRequestCookies(HttpRequestMessage request);
    }
}