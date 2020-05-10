namespace Sentinel.Domain.Models.Scan
{
    public class ScanRules
    {
        public bool AllowPassive { get; set; }

        //TODO:: Active scanning capabilities
        //TODO:: External tool integrations

        public static ScanRules DefaultSafeScanRules() => new ScanRules{ AllowPassive = true };
    }
}