using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sentinel.Domain;
using Sentinel.Domain.Models.Scan;
using Sentinel.Functions.Scanner;
using Xunit;

namespace Sentinel.Functions.Test
{
    public class PassiveScanTests
    {
        private readonly ILogger _logger = TestFactory.CreateLogger();

        [Fact]
        public async void Http_Trigger_With_Empty_Request_Returns_Bad_Request()
        {
            var request = TestFactory.CreateHttpPostRequest(string.Empty);
            var response = (BadRequestResult)await SentinelSecurityScanner.Run(request, _logger);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async void Server_With_No_Headers_Returns_Failed_Scans()
        {
            var targetResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("OK")
            };

            SentinelSecurityScanner.InjectMock<IHttpClientWrapper>(x => new MockHttpClientWrapper(targetResponse));

            // Act
            // Actual request.
            var request = TestFactory.CreateHttpPostRequest(new ScanRequest(Target.FromUri("https://domain.example.com")));
            var response = (JsonResult) await SentinelSecurityScanner.Run(request, _logger);

            // Assert
            var results = response.Value as ScanResults;
            
            Assert.NotNull(results);
            Assert.False(results.Results.All(x => x.Value.Passed));
        }
    }
}
