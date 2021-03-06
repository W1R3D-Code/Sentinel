﻿using System;
using System.Collections.Generic;
using Sentinel.Domain.Models.Authentication;

namespace Sentinel.Domain.Models.Scan
{
    public class Target
    {
        private IAuthenticationDetail _authentication;
        public string Host { get; set; }
        public string Url { get; set; }
        public bool UriIsRelative
        {
            get
            {
                if (Uri.TryCreate(Url, UriKind.Relative, out _))
                    return true;

                if (!Url.StartsWith("http") && Uri.TryCreate($"http://{Url}", UriKind.Absolute, out _))
                    return false;
                else if (Uri.TryCreate(Url, UriKind.Absolute, out _))
                    return false;
                    
                return true;
            }
        }
        public bool UseHttps { get; set; }
        public ScanRules Rules { get; set; }
        public AuthOptions AuthOptions { get; set; }
        public IAuthenticationDetail Authentication
        {
            get => _authentication ?? (_authentication = AuthOptions.ToAuthenticationDetail(Host));
            set => _authentication = value;
        }

        public Dictionary<string, string> CustomHeaders { get; set; }

        public Target() => CustomHeaders = new Dictionary<string, string>();

        public static Target FromUri(string uri, bool useHttps = true) => new Target().WithUri(uri, useHttps);
        public static Target FromUri(Uri uri, bool useHttps = true) => new Target().WithUri(uri, useHttps);

        public Target WithUri(string url, bool useHttps = true)
        {
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
                throw new ArgumentException(nameof(url));

            return this.WithUri(uri, useHttps);
        }

        public Target WithUri(Uri uri, bool useHttps = true)
        {
            Url = uri.ToString();
            UseHttps = useHttps;

            return this.WithDefaultSafeScanRules();
        }

        public Target WithDefaultSafeScanRules() => this.WithScanRules();

        /// <summary>
        /// Sets the Scan Rules, either Passive or [not implemented yet] Active profiles
        /// </summary>
        /// <param name="rules">Rule profile, defaults to passive only</param>
        /// <returns></returns>
        public Target WithScanRules(ScanRules rules = null)
        {
            Rules = rules ?? ScanRules.DefaultSafeScanRules();
            return this;
        }

        public Target WithAuthentication(IAuthenticationDetail authentication)
        {
            Authentication = authentication;
            return this;
        }

        public Target WithCustomHeaders(Dictionary<string, string> customHeaders)
        {
            CustomHeaders = customHeaders ?? new Dictionary<string, string>();
            return this;
        }
    }
}