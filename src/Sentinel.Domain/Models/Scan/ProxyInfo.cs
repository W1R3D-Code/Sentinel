using System;
using System.Net;
using Newtonsoft.Json;

namespace Sentinel.Domain.Models.Scan
{
    public class ProxyInfo
    {
        public ProxyInfo(Uri uri, string username, string password) : this(uri)
        {
            Username = username;
            Password = password;
        }
        public ProxyInfo(Uri uri)
        {
            Uri = uri;
        }

        [JsonConstructor]
        public ProxyInfo(string uri, int port, string username = null, string password = null)
        {
            Uri = uri.StartsWith("http://") ? 
                new Uri($"{uri}:{port}") :
                new Uri($"http://{uri}:{port}");
            
            Username = username;
            Password = password;
        }

        public Uri Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public WebProxy ToWebProxy()
        {
            var proxy = new WebProxy(Uri, false);

            if (string.IsNullOrEmpty(Username))
                proxy.Credentials = new NetworkCredential(Username, Password);

            return proxy;
        }
    }
}