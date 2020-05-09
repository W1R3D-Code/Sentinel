using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Scanner
{
    public abstract class PassiveScannerBase : IPassiveScanner
    {
        private readonly ILogger _logger;

        protected PassiveScannerBase(ILoggerFactory loggerFactory) =>
            _logger = loggerFactory.CreateLogger(this.GetType().Name);
        
        public abstract string Name();
        public abstract string Details();
        public abstract Severity DefaultSeverity();
        public abstract ScanResult Run(HttpResponseMessage response);
        public abstract bool IsEnabled();
    }
}