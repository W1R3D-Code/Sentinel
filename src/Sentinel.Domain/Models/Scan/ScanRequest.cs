using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;

namespace Sentinel.Domain.Models.Scan
{
    public class ScanRequest
    {
        public string Host { get; set; }
        public List<Target> Targets { get; set; }
        public bool AllowTargetsWithDifferentHostName { get; set; }
        public ProxyInfo Proxy { get; set; }
        public ScanRules DefaultRules { get; set; }

        public ScanRequest(Target target, string host = null, bool allowTargetsWithDifferentHost = false) :
            this(new List<Target> {target}, host, allowTargetsWithDifferentHost) { }

        [JsonConstructor]
        public ScanRequest(List<Target> targets, string host = null, bool allowTargetsWithDifferentHost = false, AuthOptions authOptions = null, ProxyInfo proxy = null, ScanRules rules = null) : this(host)
        {
            if (targets.Any(x => x.UriIsRelative) && string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host));

            DefaultRules = rules ?? ScanRules.DefaultSafeScanRules();

            Targets = ApplyGlobalDefaultsToTargets(targets, host, authOptions);

            AllowTargetsWithDifferentHostName = allowTargetsWithDifferentHost;
            Proxy = proxy;
        }

        public ScanRequest(string host) => Host = host;

        /// <summary>
        /// Apply inheritance of optional properties where omitted on target
        /// </summary>
        /// <param name="targets">List of Targets</param>
        /// <param name="host">Host name</param>
        /// <param name="authOptions">Authentication Options</param>
        private List<Target> ApplyGlobalDefaultsToTargets(List<Target> targets, string host, AuthOptions authOptions = null)
        {
            SetAuthenticationForTargets(targets, authOptions, host);
            SetHostForTargets(targets, host);
            SetRulesForTargets(targets);

            return targets;
        }

        private static void SetAuthenticationForTargets(List<Target> targets, AuthOptions authOptions, string host)
        {
            if (authOptions == null || targets.All(x => x.Authentication != null))
                return;

            var authDetails = authOptions.ToAuthenticationDetail(host);

            foreach (var target in targets.Where(x => x.Authentication == null))
                target.Authentication = authDetails;
        }

        private static void SetHostForTargets(List<Target> targets, string host)
        {
            if (!string.IsNullOrWhiteSpace(host) && targets.Any(x => x.Host == null))
                foreach (var target in targets.Where(x => x.Host == null))
                    target.Host = host;
        }

        private static void SetRulesForTargets(List<Target> targets)
        {
            if (targets.Any(x => x.Rules == null))
                foreach (var target in targets.Where(x => x.Rules == null))
                    target.Rules = ScanRules.DefaultSafeScanRules();

            return;
        }
    }
}
