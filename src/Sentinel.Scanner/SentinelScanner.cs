using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sentinel.Domain;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Scanner
{
    public class SentinelScanner : ISentinelScanner
    {
        private readonly ILogger _logger;
        private readonly HttpClientWrapper _httpClientWrapper;
        private readonly IEnumerable<IPassiveScanner> _passiveScanners;

        public SentinelScanner(ILoggerFactory logger, IEnumerable<IPassiveScanner> passiveScanners, HttpClient httpClient)
        {
            _logger = logger.CreateLogger(nameof(SentinelScanner));
            _passiveScanners = passiveScanners;
            _httpClientWrapper = new HttpClientWrapper(logger, httpClient);
        }

        public async Task<ScanResults> RunAsync(ScanRequest request, CancellationToken cancellationToken)
        {
            // TODO:: Inject HttpClient or Requester singleton
            var results = new ScanResults(request.HostName);

            // TODO:: Option to run in parallel
            foreach (var target in request.Targets)
            {
                var rules = target.Rules;
                var requestTarget = target.ToGetRequestMessage(request.HostName);

                if (rules.AllowPassive)
                {
                    var response = await _httpClientWrapper.TrySendRequest(requestTarget, cancellationToken);

                    // TODO:: Run in parallel
                    results.Results.AddRange(from scanner in _passiveScanners where scanner.IsEnabled() select scanner.Run(response));
                }
            }

            return results;
        }
    }
}
