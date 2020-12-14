using System.Threading;
using System.Threading.Tasks;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Scanner
{
    public interface ISentinelScanner
    {
        Task<ScanResults> RunAsync(ScanRequest request, CancellationToken cancellationToken);
    }
}