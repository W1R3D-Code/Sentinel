using System.Collections.Generic;

namespace Sentinel.Domain.Models
{
    public class Resource
    {
        public string Uri { get; set; }
        public ResourceType ResourceType { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
    }
}