using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;
using Sentinel.Scanner;

namespace Sentinel.Cli
{
    public class Program
    {
        private static ServiceProvider _serviceProvider;
        private static ILogger<Program> _logger;

        static void Main(string[] args)
        {
            if (args.Length < 1)
                throw new ArgumentException();

            ConfigureServices();

            var scanner = _serviceProvider.GetService<ISentinelScanner>();

            var scanRequest = new ScanRequest
            {
                Targets = new List<Target>
                {
                    new Target
                    {
                        Url = args[0],
                        Rules = new ScanRules {AllowPassive = true},
                    }
                }
            };

            _logger.Log(LogLevel.Trace, $"Scanning {args[0]}");

            var scanResults = scanner.RunAsync(scanRequest, CancellationToken.None).Result;
            WriteSummaryAndResultsToConsole(scanResults);
        }

        private static void ConfigureServices()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging(x => x
                    .AddFilter("Sentinel.Cli.Program", LogLevel.Debug)
                    .AddConsole())
                .AddSingleton<IPassiveScanner, ClickjackingScanner>()
                .AddSingleton<ISentinelScanner>(x => new SentinelScanner(x.GetService<ILoggerFactory>(),
                    x.GetService<IEnumerable<IPassiveScanner>>(), new HttpClient()))
                .BuildServiceProvider();

            _logger = _serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
        }

        private static void WriteSummaryAndResultsToConsole(ScanResults scanResults)
        {
            var summary = scanResults.Summary.ToList().OrderByDescending(x => x.Key);

            foreach (var (severity, count) in summary)
                Console.WriteLine($"{severity}: {count}");

            foreach (var r in scanResults.Results.OrderByDescending(x => (int) x.Severity))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Scan: {r.Scan.Name}");
                Console.WriteLine($"Details: {r.Scan.Details}");
                Console.WriteLine($"Result: {(r.Passed ? "Passed" : $"Failed ({r.Severity})")}");
                Console.WriteLine();
                foreach (var f in r.Failures)
                {
                    Console.WriteLine(
                        $"Failed On: {f.Message}{(f.FailingItems.Any() ? string.Join(", ", f.FailingItems.Where(x => !string.IsNullOrWhiteSpace(x))) : string.Empty)}");
                }
            }
        }
    }
}
