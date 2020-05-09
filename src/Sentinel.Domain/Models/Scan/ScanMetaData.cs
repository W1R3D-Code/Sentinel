namespace Sentinel.Domain.Models.Scan
{
    public class ScanMetaData
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public Severity Severity { get; set; }

        public ScanMetaData(string name, string details, Severity severity = Severity.Medium)
        {
            Name = name;
            Details = details;
            Severity = severity;
        }
    }
}