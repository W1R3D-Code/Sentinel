using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;

namespace Sentinel.Domain.Models.Authentication
{
    public interface IAuthenticationDetail
    {
        void PreRequestAction();
        void AddRequestAuthentication(HttpRequestMessage request);
    }
}