using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sentinel.Domain;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models.Scan;
using Sentinel.Scanner;
using Sentinel.Scanner.Helpers;

[assembly: InternalsVisibleTo("Sentinel.Functions.Test")]
namespace Sentinel.Functions.Scanner
{
    public static class SentinelSecurityScanner
    {
        private static ILogger _log;
        private static ServiceProvider _serviceProvider;
        private static readonly IServiceCollection ServiceCollection;

        static SentinelSecurityScanner()
        {
            ServiceCollection = new ServiceCollection()
                .AddLogging(x => x
                    .AddFilter("Sentinel.Functions.Scanner.SentinelSecurityScanner", LogLevel.Debug))
                .RegisterAllImplementations<IPassiveScanner>()
                .AddSingleton<IHttpClientWrapper>(x => new HttpClientWrapper(x.GetService<ILoggerFactory>()))
                .AddSingleton(
                    x => new RequestHelper(x.GetService<IHttpClientWrapper>(), x.GetService<ILoggerFactory>()))
                .AddTransient<ISentinelScanner>(x => new SentinelScanner(x.GetService<ILoggerFactory>(),
                    x.GetService<IEnumerable<IPassiveScanner>>(), x.GetService<IHttpClientWrapper>(),
                    x.GetService<RequestHelper>()));

            _serviceProvider = ServiceCollection.BuildServiceProvider();
        }

        [FunctionName("Scan")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            _log = log;
            _log.BeginScope($"{nameof(SentinelSecurityScanner)}.{nameof(Run)}");

            var request = await TryParseRequest(req.Body);

            if (request == null)
                return new BadRequestResult();

            try
            {
                var scanner = _serviceProvider.GetService<ISentinelScanner>();
                var scanResults = await scanner.RunAsync(request, CancellationToken.None);

                _log.Log(LogLevel.Information,
                    scanResults.HasFailures()
                        ? "Scan returned with failing tests."
                        : "Scan returned with no failing tests.");

                return new JsonResult(scanResults);
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e.Message);
                return new BadRequestResult();
            }
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
