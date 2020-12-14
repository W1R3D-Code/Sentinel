using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sentinel.Domain.Models;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Test.ExtensionTests
{
    public class ScanResultExtensionsTests
    {
        private ScanResults _scanResults;

        [SetUp]
        public void SetUp()
        {
            var results = new ConcurrentDictionary<string, ScanResult>();
            results.TryAdd("Test1", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Critical)));
            results.TryAdd("Test2", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.High)));
            results.TryAdd("Test3", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.High)));
            results.TryAdd("Test4", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Medium)));
            results.TryAdd("Test5", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Medium)));
            results.TryAdd("Test6", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Medium)));
            results.TryAdd("Test7", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Low)));
            results.TryAdd("Test8", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Low)));
            results.TryAdd("Test9", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Low)));
            results.TryAdd("Test10", new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Low)));

            _scanResults = new ScanResults(string.Empty) {Results = results};
        }

        [Test]
        public void Summary_Counts_All_Severities_In_Result_Set()
        {
            // Given
            _scanResults.Results.Select(x => x.Value)
                .ToList().ForEach(x => x.AddFailure(string.Empty));

            // Act ScanResults.Summary
            var summary = _scanResults.Summary;

            // Then
            var severities = Enum.GetValues(typeof(Severity));

            summary.Should().NotBeEmpty();
            summary.Count().Should().Be(severities.Length);

            summary[Severity.Critical].Should().Be(1);
            summary[Severity.High].Should().Be(2);
            summary[Severity.Medium].Should().Be(3);
            summary[Severity.Low].Should().Be(4);
            summary[Severity.Informative].Should().Be(0);
        }
    }
}
