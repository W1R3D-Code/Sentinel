using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using FluentAssertions;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Sentinel.Domain;
using Sentinel.Domain.Models.Scan;
using Sentinel.Scanner;

namespace Sentinel.Test
{
    [TestFixture]
    public class ClickJackingTests
    {
        private ILoggerFactory _loggerFactory; 

        [SetUp]
        public void Setup()
        {
            _loggerFactory = A.Fake<ILoggerFactory>();
        }

        [Test]
        public void Fails_Test_And_Warns_If_CSP_and_xFrameOptions_Not_Set()
        {
            // Given
            var response = new HttpResponseMessage();
            var scanner = new ClickjackingScanner(_loggerFactory);
            
            // Act
            var results = scanner.Run(response);

            // Then
            results.Passed.Should().BeFalse();
            results.Failures.Should().NotBeEmpty();
            results.Failures.First().Message.Should().Be("Neither X-Frame-Options or Content-Security-Policy are not set.");
        }

        [Test]
        public void Fails_Test_And_Warns_If_xFrameOptions_Allows_Any()
        {
            // Given
            var response = new HttpResponseMessage();
            response.Headers.Add(HeaderNames.XFrameOptions, "Allow-From *");
            var scanner = new ClickjackingScanner(_loggerFactory);

            // Act
            var results = scanner.Run(response);

            // Then
            results.Passed.Should().BeFalse();
            results.Failures.Should().NotBeEmpty();
            results.Failures.First().Message.Should().Be("X-Frame-Options is set but allows any domain");
        }

        [Test]
        public void Fails_Test_And_Warns_When_Csp_Frame_Ancestors_Allows_Any()
        {
            // Given
            var response = new HttpResponseMessage();
            var scanner = new ClickjackingScanner(_loggerFactory);
            response.Headers.Add(HeaderNames.ContentSecurityPolicy, "frame-ancestors *");

            // Act
            var results = scanner.Run(response);

            // Then
            results.Passed.Should().BeFalse();
            results.Failures.Should().NotBeEmpty();
            results.Failures.First().Message.Should().Be("Content-Security-Policy frame-ancestors directive allows any domain.");
        }
    }
}
