using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Sentinel.Domain.Extensions
{
    public static class HttpHeadersExtensions
    {
        public static Dictionary<string, List<string>> GetCspDirectives(this HttpHeaders headers)
        {
            var directives = new Dictionary<string, List<string>>();
            
            if (!headers.TryGetValues(HeaderNames.ContentSecurityPolicy, out var cspValues))
                return directives;
            
            foreach (var cspValue in cspValues.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                foreach (var directive in cspValue.Split(new []{ ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var splitDirective = directive?.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var directiveName = splitDirective?.FirstOrDefault();
                    var directiveSource = splitDirective.Skip(1).ToList();

                    if (string.IsNullOrWhiteSpace(directiveName))
                        continue;

                    if (directives.ContainsKey(directiveName))
                        directives[directiveName].AddRange(directiveSource);
                    else
                        directives.Add(directiveName, directiveSource);
                }   
            }

            return directives;
        }
    }
}
