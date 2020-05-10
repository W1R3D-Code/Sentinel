using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Sentinel.Domain.Models.Scan;
using Sentinel.Functions.PassiveScan;
using Sentinel.Scanner;
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
            var response = (BadRequestResult)await SentinelPassiveScan.Run(request, _logger);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async void Server_With_No_Headers_Returns_Failed_Scans()
        {
            var targetResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("OK")
            };

            SentinelPassiveScan.InjectMock<IHttpClientWrapper>(x => new MockHttpClientWrapper(targetResponse));

            // Act
            // Actual request.
            var request = TestFactory.CreateHttpPostRequest(new ScanRequest(Target.FromUri("https://domain.example.com")));
            var response = (JsonResult) await SentinelPassiveScan.Run(request, _logger);

            // Assert
            var results = response.Value as ScanResults;
            
            Assert.NotNull(results);
            Assert.False(results.Results.All(x => x.Passed));
        }
    }
}
