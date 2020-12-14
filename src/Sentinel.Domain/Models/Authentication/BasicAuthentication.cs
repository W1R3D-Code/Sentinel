using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Domain.Models.Authentication
{
    public class BasicAuthentication : IAuthenticationDetail
    {
        private readonly string _username;
        private readonly string _password;
        private const string HeaderName = "Authorization";
        private string HeaderValue => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"))}";

        public BasicAuthentication(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public async Task PreRequestAction(IHttpClientWrapper client) => await Task.CompletedTask;

        public void AddRequestCookies(HttpRequestMessage request)
        {
            request.Headers.Add(HeaderName, HeaderValue);
        }
    }
}