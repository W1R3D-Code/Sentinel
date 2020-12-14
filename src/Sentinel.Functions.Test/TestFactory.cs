using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Functions.Test
{
    public class TestFactory
    {
        public static ScanRequest Data(string url) => new ScanRequest(new Target().WithUri(url));

        public static DefaultHttpRequest CreateHttpPostRequest(ScanRequest request) =>
            TestFactory.CreateHttpPostRequest(JsonSerializer.Serialize(request));

        public static DefaultHttpRequest CreateHttpPostRequest(string body) =>
            new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = body.ToStream() // Disposing in this scope will make the stream un-readable 
            };

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null) => type == LoggerTypes.List ? new ListLogger() : NullLoggerFactory.Instance.CreateLogger("Null Logger");
    }
}