using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _scanResults = new ScanResults(string.Empty)
            {
                Results = new List<ScanResult>
                {
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Critical)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.High)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.High)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Medium)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Medium)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Medium)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Low)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Low)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Low)),
                    new ScanResult(new ScanMetaData(string.Empty, string.Empty, Severity.Low)),
                }
            };
        }

        [Test]
        public void Summary_Counts_All_Severities_In_Result_Set()
        {
            // Given
            _scanResults.Results.ForEach(x => x.AddFailure(string.Empty));

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
