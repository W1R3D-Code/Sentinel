using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Sentinel.Domain;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models.Scan;
using Sentinel.Domain.Models.Authentication;

namespace Sentinel.Test
{
    public class CustomHeaderTests
    {
        [Test]
        public void When_Custom_Headers_Are_Specified_They_Are_Added_To_Request()
        {
            var customHeaders = new Dictionary<string, string>
            {
                {"SessionId", "jor29ojt7nrrsmtziu1kublv"},
                {"__RequestVerificationToken", "iZUhAcV0-PceeJv-pNf5ap4I4zvPcQd4jgLx2wIFyVKTBdp-t33LzrmRwg-U1y3GPypmxL3MyT7o-b4K5SlOhOln2ymlgQR0rXpL_ddvS0g1"}
            };

            var target = Target.FromUri("https://domain.example.com")
                .WithCustomHeaders(customHeaders);

            var request = target.ToGetRequestMessage();

            var headers = request.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());

            headers.Should().NotBeEmpty();
            headers.Should().BeEquivalentTo(customHeaders);
        }
    }
}
