using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Sentinel.Domain.Models.Authentication
{
    public class FormBasedAuthentication : IAuthenticationDetail
    {
        private readonly Uri _uri;
        private Uri _antiCsrfUri;
        private readonly string _username;
        private readonly string _password;

        public Uri AntiCsrfUri
        {
            get => _antiCsrfUri ?? _uri;
            set => _antiCsrfUri = value;
        }
        public string AntiCsrfTokenName { get; set; }
        public RequestEncoding RequestEncoding { get; set; }

        private Dictionary<string, string> _authCookies;

        public FormBasedAuthentication(Uri uri, string username, string password)
        {
            _uri = uri;
            _username = HttpUtility.UrlEncode(username);
            _password = HttpUtility.UrlEncode(password);
            _authCookies = new Dictionary<string, string>();
        }
        
        public async Task PreRequestAction(IHttpClientWrapper client)
        {
            string antiCsrfHeader = null;
            string antiCsrfFormToken = null;

            if (AntiCsrfUri != null && !string.IsNullOrWhiteSpace(AntiCsrfTokenName))
                (antiCsrfHeader, antiCsrfFormToken) = await GetAntiCrossSiteRequestForgeryTokenPair(client);

            var authRequest = new HttpRequestMessage(HttpMethod.Post, _uri);

            AddRequestCookies(authRequest);
            authRequest.Content = GetAuthRequestContent(antiCsrfFormToken);

            var response = await client.TrySendRequest(authRequest, CancellationToken.None);
            
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Redirect)
                throw new Exception("Authentication request failed");

            foreach (var cookie in GetAllCookiesFromResponse(response))
                _authCookies[cookie.Key] = cookie.Value;
        }

        private StringContent GetAuthRequestContent(string antiCsrfToken)
        {
            switch (RequestEncoding)
            {
                case RequestEncoding.FormUrlEncoding:
                    return new StringContent($"username={_username}&password={_password}{(string.IsNullOrWhiteSpace(antiCsrfToken) ? string.Empty : $"&{antiCsrfToken}")}");
                case RequestEncoding.MultiPartFormData:
                    throw new NotImplementedException(nameof(RequestEncoding.MultiPartFormData));
                case RequestEncoding.Json:
                    return new StringContent(
                        $"{{\r\n\t\"username\":\"{_username}\"\r\n\t\"password\":\"{_password}\"{(string.IsNullOrWhiteSpace(antiCsrfToken) ? string.Empty : $"\r\n\t\"{antiCsrfToken}\"")}\r\n}}");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<(string header, string token)> GetAntiCrossSiteRequestForgeryTokenPair(IHttpClientWrapper client)
        {

            var response = await client.TrySendRequest(new HttpRequestMessage(HttpMethod.Get, AntiCsrfUri), CancellationToken.None);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Redirect)
                throw new Exception("Failed to load page requesting new Anti-CSRF token");

            // Things like Session cookies can get set on this response,
            // and if not updated will not be visible on the actual auth response
            foreach (var cookie in GetAllCookiesFromResponse(response))
                _authCookies[cookie.Key] = cookie.Value;

            var header = GetCookie(response, AntiCsrfTokenName);

            if (string.IsNullOrWhiteSpace(header))
                throw new Exception("Can't find Anti-CSRF Token Header");

            var token = string.Empty;

            var bodyRegex = new Regex($"[\"']{{1}}{AntiCsrfTokenName}[\"']{{1}}.+?(?=value=)value=[\"']{{1}}([\\S]+)[\"']{{1}}");

            var content = await response.Content.ReadAsStringAsync();
            foreach (Match match in bodyRegex.Matches(content))
            {
                token = match.Groups[1].Value;
                break;
            }

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Can't find Anti-CSRF Token in response body");

            return (header, token);
        }

        public void AddRequestCookies(HttpRequestMessage request)
        {
            foreach (var cookie in _authCookies)
            {
                request.Headers.TryAddWithoutValidation(cookie.Key, cookie.Value);
            }
        }

        private string GetCookie(HttpResponseMessage response, string cookieName)
        {
            var cookie = response.Headers
                .LastOrDefault(header => header.Key == "Set-Cookie" && header.Value.Any(y => y.StartsWith(cookieName)))
                .Value
                .LastOrDefault(x => x.StartsWith(cookieName));

            return !string.IsNullOrEmpty(cookie) && cookie.Contains($"{cookieName}=")
                ? cookie
                    .Substring(cookie.IndexOf('=') + 1)
                    .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
                    ?.TrimEnd()
                : string.Empty;
        }

        private Dictionary<string, string> GetAllCookiesFromResponse(HttpResponseMessage response)
        {
            var cookies = new Dictionary<string, string>();

            var headerValues = response.Headers
                .Where(header => header.Key == "Set-Cookie")
                .SelectMany(x => x.Value);

            foreach (var headerValue in headerValues)
            {
                var (name, value) = GetCookieFromSetter(headerValue);
                cookies[name] = value;
            }

            return cookies;
        }

        private (string name, string value) GetCookieFromSetter(string headerValue)
        {
            var components = headerValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var nameAndValue = components.First().Split(new []{'='}, StringSplitOptions.RemoveEmptyEntries);
            return (nameAndValue.First(), nameAndValue.LastOrDefault());
        }
    }
}