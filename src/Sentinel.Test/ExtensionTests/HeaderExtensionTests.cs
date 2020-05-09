using System.Collections.Generic;
using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;
using Sentinel.Domain.Extensions;

namespace Sentinel.Test.ExtensionTests
{
    public class HeaderExtensionTests
    {
        [Test]
        public void Should_Get_Correct_Csp_Directives()
        {
            // Given
            var response = new HttpResponseMessage();
            response.Headers.Add("Content-Security-Policy",
                "default-src 'none'; script-src 'self' 'nonce-r@nd0m'; connect-src 'self'; img-src 'self' domain.example.com; style-src 'self';base-uri 'self';form-action 'self'");


            // Act
            var directives = response.Headers.GetCspDirectives();
            

            // Then
            directives.Should().NotBeEmpty();
            directives.Should().ContainKeys(new[] { "default-src", "script-src", "connect-src", "img-src", "style-src", "base-uri", "form-action" });

            directives.Should().BeEquivalentTo(new Dictionary<string, List<string>>()
            {
                { "default-src", new List<string> { "'none'" } },
                { "script-src", new List<string> { "'self'", "'nonce-r@nd0m'" } },
                { "connect-src", new List<string> { "'self'" } },
                { "img-src", new List<string> { "'self'", "domain.example.com" } },
                { "style-src", new List<string> { "'self'" } },
                { "base-uri", new List<string> { "'self'" } },
                { "form-action", new List<string> { "'self'" } }
            });
        }
    }
}
