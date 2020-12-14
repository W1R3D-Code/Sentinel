using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Sentinel.Domain.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static CookieCollection GetCookiesSetOnResponse(this HttpResponseMessage response)
        {
            CookieCollection collection = null;

            foreach (var header in response.Headers.Where(x =>
                string.Equals(x.Key, "Set-Cookie", StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var setCookie in header.Value)
                {
                    collection = GetAllCookiesFromHeader(setCookie);
                }
            }

            return collection;
        }

        public static CookieCollection GetAllCookiesFromHeader(string header)
        {
            if (string.IsNullOrWhiteSpace(header))
                throw new ArgumentNullException(nameof(header));

            var cookieList = ConvertCookieHeaderToArrayList(header);
            return ConvertCookieArraysToCookieCollection(cookieList);
        }


        private static ArrayList ConvertCookieHeaderToArrayList(string cookieHeader)
        {
            cookieHeader = cookieHeader.Replace("\r", "");
            cookieHeader = cookieHeader.Replace("\n", "");
            var cookieParts = cookieHeader.Split(',');
            var list = new ArrayList();
            
            for (var i = 0; i < cookieParts.Length; i++)
            {
                if (cookieParts[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                    list.Add(cookieParts[i] + "," + cookieParts[i++]);
                else
                    list.Add(cookieParts[i]);
            }

            return list;
        }


        private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList cookies)
        {
            var cookieCollection = new CookieCollection();

            foreach (string cookie in cookies)
            {
                var cookieDirectives = cookie.Split(';');
                var intEachCookPartsCount = cookieDirectives.Length;
                var cookTemp = new Cookie();

                for (var j = 0; j < intEachCookPartsCount; j++)
                {
                    if (j == 0)
                    {
                        var nameAndValue = cookieDirectives[j];
                        if (nameAndValue != string.Empty)
                        {
                            var firstEqual = nameAndValue.IndexOf("=", StringComparison.Ordinal);
                            var firstName = firstEqual >= 0 ? nameAndValue.Substring(0, firstEqual) : nameAndValue;
                            var allValue = nameAndValue.Substring(firstEqual + 1, nameAndValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }
                        continue;
                    }

                    string[] keyValuePair;
                    string pathAndValue;
                    if (cookieDirectives[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        pathAndValue = cookieDirectives[j];
                        if (pathAndValue != string.Empty)
                        {
                            keyValuePair = pathAndValue.Split('=');
                            cookTemp.Path = keyValuePair[1] != string.Empty ? keyValuePair[1] : "/";
                        }
                        continue;
                    }

                    if (cookieDirectives[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        pathAndValue = cookieDirectives[j];
                        if (pathAndValue != string.Empty)
                        {
                            keyValuePair = pathAndValue.Split('=');
                            if (!string.IsNullOrWhiteSpace(keyValuePair[1]))
                                cookTemp.Domain = keyValuePair[1];
                        }
                    }
                }

                if (cookTemp.Path == string.Empty)
                    cookTemp.Path = "/";

                cookieCollection.Add(cookTemp);
            }

            return cookieCollection;
        }
    }
}
