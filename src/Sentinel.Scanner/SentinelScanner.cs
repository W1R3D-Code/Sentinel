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
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly IEnumerable<IPassiveScanner> _passiveScanners;

        public SentinelScanner(ILoggerFactory logger, IEnumerable<IPassiveScanner> passiveScanners, IHttpClientWrapper httpClientWrapper)
        {
            _logger = logger.CreateLogger(nameof(SentinelScanner));
            _passiveScanners = passiveScanners;
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<ScanResults> RunAsync(ScanRequest request, CancellationToken cancellationToken)
        {
            // TODO:: Inject HttpClient or Requester singleton
            var results = new ScanResults(request.Host);

            // TODO:: Option to run in parallel
            foreach (var target in request.Targets)
            {
                var rules = target.Rules;
                var requestTarget = target.ToGetRequestMessage(request.Host);

                if (rules.AllowPassive)
                {
                    var response = await _httpClientWrapper.TrySendRequest(requestTarget, cancellationToken);

                    // TODO:: Run in parallel
                    if (response != null)
                        results.Results.AddRange(from scanner in _passiveScanners where scanner.IsEnabled() select scanner.Run(response));
                }
            }

            return results;
        }
    }
}
