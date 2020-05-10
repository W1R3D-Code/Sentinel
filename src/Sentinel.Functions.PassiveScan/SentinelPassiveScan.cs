using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sentinel.Domain.Models.Scan;
using Sentinel.Scanner;

[assembly: InternalsVisibleTo("Sentinel.Functions.Test")]
namespace Sentinel.Functions.PassiveScan
{
    public static class SentinelPassiveScan
    {
        private static ILogger _log;
        private static ServiceProvider _serviceProvider;
        private static readonly IServiceCollection ServiceCollection;

        static SentinelPassiveScan()
        {
            ServiceCollection = new ServiceCollection()
                .AddLogging(x => x
                    .AddFilter("Sentinel.Functions.PassiveScan.SentinelPassiveScan", LogLevel.Debug))
                .AddTransient<IPassiveScanner, ClickjackingScanner>()
                .AddTransient<IHttpClientWrapper>(x => new HttpClientWrapper(x.GetService<ILoggerFactory>(), new HttpClient()))
                .AddTransient<ISentinelScanner>(x => new SentinelScanner(x.GetService<ILoggerFactory>(),
                    x.GetService<IEnumerable<IPassiveScanner>>(), x.GetService<IHttpClientWrapper>()));

            _serviceProvider = ServiceCollection.BuildServiceProvider();
        }

        [FunctionName("ScanUri")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            _log = log;
            var request = await TryParseRequest(req.Body);

            if (request == null)
                return new BadRequestResult();

            var scanner = _serviceProvider.GetService<ISentinelScanner>();
            var scanResults = await scanner.RunAsync(request, CancellationToken.None);

            return new JsonResult(scanResults);
        }

        private static async Task<ScanRequest> TryParseRequest(Stream requestBody)
        {
            var body = await new StreamReader(requestBody).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
                return null;

            try
            {
                return JsonConvert.DeserializeObject<ScanRequest>(body);
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, "Error parsing request", e);
                return null;
            }
        }

        internal static void InjectMock<TInterface>(Func<IServiceProvider, TInterface> implementationFactory) where TInterface : class
        {
            ServiceCollection.AddTransient<TInterface>(implementationFactory);
            _serviceProvider = ServiceCollection.BuildServiceProvider();
        }
    }
}
