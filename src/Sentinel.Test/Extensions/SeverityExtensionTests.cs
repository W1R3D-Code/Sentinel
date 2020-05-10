using FluentAssertions;
using NUnit.Framework;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models;

namespace Sentinel.Test.ExtensionTests
{
    public class SeverityExtensionTests
    {
        [Test]
        public void Should_Elevate_Severity_When_New_Severity_Is_Higher()
        {
            // Given
            var severity = Severity.Medium;
            var newSeverity = Severity.High;

            // Act
            severity = severity.ElevateSeverityIfApplicable(newSeverity);

            // Then
            severity.Should().Be(newSeverity);
        }
    }
}
