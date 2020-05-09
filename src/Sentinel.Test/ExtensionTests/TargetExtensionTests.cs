using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Test.ExtensionTests
{
    public class TargetExtensionTests
    {
        [Test]
        public void Request_Has_Correct_Uri_When_Target_Uri_Is_Fully_Qualified()
        {
            // Given
            var target = new Target
            {
                Url = "https://domain.example.com",
                UriIsRelative = false,
                CustomHeaders = null,
                Rules = new ScanRules
                {
                    AllowPassive = true
                },
                Authentication = null
            };

            // Act
            var request = target.ToGetRequestMessage();

            // Then
            request.RequestUri.ToString().TrimEnd('/')
                .Should().BeEquivalentTo(target.Url.TrimEnd('/'));
        }

        [Test]
        public void Request_Has_Correct_Schema_And_Uri_When_Target_Uri_Is_Fully_Qualified_Http_And_UseHttps_Is_True()
        {
            // Given
            var target = new Target
            {
                Url = "http://domain.example.com",
                UriIsRelative = false,
                UseHttps = true,
                CustomHeaders = null,
                Rules = new ScanRules
                {
                    AllowPassive = true
                },
                Authentication = null
            };

            // Act
            var request = target.ToGetRequestMessage();

            // Then
            request.RequestUri.ToString().TrimEnd('/')
                .Should().NotBe(target.Url.TrimEnd('/'));

            request.RequestUri.ToString().TrimEnd('/')
                .Should().BeEquivalentTo(target.Url.TrimEnd('/').Replace("http://", "https://"));
        }

        [Test]
        public void Request_Has_Correct_Uri_When_Target_Uri_Is_Relative()
        {
            // Given
            var target = new Target
            {
                Url = "/Test/Endpoint",
                UriIsRelative = true,
                UseHttps = false,
                CustomHeaders = null,
                Rules = new ScanRules
                {
                    AllowPassive = true
                },
                Authentication = null
            };

            var scanRequest = new ScanRequest
            {
                HostName = "domain.example.com",
                Targets = new List<Target> { target }
            };

            // Act
            var request = target.ToGetRequestMessage(scanRequest.HostName);

            // Then
            request.RequestUri.ToString().TrimEnd('/')
                .Should().BeEquivalentTo($"http://{scanRequest.HostName}/{target.Url.Trim('/')}");
        }

        [Test]
        public void Request_Has_Correct_Schema_And_Uri_When_Target_Uri_Is_Relative_And_UseHttps_Is_True()
        {
            // Given
            var target = new Target
            {
                Url = "/Test/Endpoint",
                UriIsRelative = true,
                UseHttps = true,
                CustomHeaders = null,
                Rules = new ScanRules
                {
                    AllowPassive = true
                },
                Authentication = null
            };

            var scanRequest = new ScanRequest
            {
                HostName = "domain.example.com",
                Targets = new List<Target> { target }
            };

            // Act
            var request = target.ToGetRequestMessage(scanRequest.HostName);

            // Then
            request.RequestUri.ToString().TrimEnd('/')
                .Should().BeEquivalentTo($"https://{scanRequest.HostName}/{target.Url.Trim('/')}");
        }

        [Test]
        public void Should_Throw_Arugment_Exception_When_Relative_Url_And_No_Host()
        {
            // Given
            var target = new Target
            {
                Url = "/Test/Endpoint",
                UriIsRelative = true,
                UseHttps = false,
                CustomHeaders = null,
                Rules = new ScanRules
                {
                    AllowPassive = true
                },
                Authentication = null
            };

            // Act
            Action act = () => target.ToGetRequestMessage(null);

            // Then
            act.Should().ThrowExactly<ArgumentNullException>()
                .And.ParamName.Should().Be("host");
        }
    }
}
