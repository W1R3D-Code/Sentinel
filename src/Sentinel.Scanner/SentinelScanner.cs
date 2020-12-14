using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sentinel.Domain;
using Sentinel.Domain.Models.Scan;
using Sentinel.Scanner.Helpers;

namespace Sentinel.Scanner
{
    public class SentinelScanner : ISentinelScanner
    {
        private readonly ILogger _logger;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly RequestHelper _requestHelper;
        private readonly IEnumerable<IPassiveScanner> _passiveScanners;

        public SentinelScanner(ILoggerFactory logger, IEnumerable<IPassiveScanner> passiveScanners, IHttpClientWrapper httpClientWrapper, RequestHelper requestHelper)
        {
            _logger = logger.CreateLogger(nameof(SentinelScanner));
            _passiveScanners = passiveScanners;
            _httpClientWrapper = httpClientWrapper;
            _requestHelper = requestHelper;
        }

        public async Task<ScanResults> RunAsync(ScanRequest request, CancellationToken cancellationToken)
        {
            // Important, as the HttpClient can not be edited after initializing
            _httpClientWrapper.RegisterClient(request.Proxy, false);
            var results = new ScanResults(request.Host);

            var tasks = request.Targets.Select(target => ScanTarget(request, cancellationToken, target, results))
                .ToList();

            await Task.WhenAll(tasks.ToArray());

            return results;
        }

        private async Task ScanTarget(ScanRequest request, CancellationToken cancellationToken, Target target,
            ScanResults results)
        {
            var rules = target.Rules;
            var requestMessage = await _requestHelper.ToGetRequestMessageAsync(target, request.Host);

            if (rules.AllowPassive)
            {
                var response = await _httpClientWrapper.TrySendRequest(requestMessage, cancellationToken);

                if (response != null)
                {
                    var tasks = _passiveScanners.Where(x => x.IsEnabled()).Select(passiveScanner =>
                        RunScanner(results, passiveScanner, response)).ToList();

                    await Task.WhenAll(tasks.ToArray());
                }
            }
        }

        private Task RunScanner(ScanResults results, IPassiveScanner passiveScanner, HttpResponseMessage response)
        {
            var result = passiveScanner.Run(response);
            results.Results.AddOrUpdate(passiveScanner.Name(), result, (key, oldVal) => result);
            return Task.CompletedTask;
        }
    }
}
