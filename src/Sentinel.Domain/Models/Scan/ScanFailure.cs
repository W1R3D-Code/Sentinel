using System.Collections.Generic;

namespace Sentinel.Domain.Models.Scan
{
    public class ScanFailure
    {
        public string Message { get; set; }
        public IEnumerable<string> FailingItems { get; set; }

        public ScanFailure(string message, IEnumerable<string> failingItems = null)
        {
            Message = message;
            FailingItems = failingItems ?? new List<string>();
        }
    }
}