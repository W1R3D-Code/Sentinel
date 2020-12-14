using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Sentinel.Domain.Models.Scan;
using Sentinel.Scanner.Helpers;

namespace Sentinel.Test.Helpers
{
    public class RequestHelperTests : TestBase
    {
        [Test]
        public void Request_Has_Correct_Uri_When_Target_Uri_Is_Fully_Qualified()
        {
            // Given
            var target = Target.FromUri("https://domain.example.com");

            // Act
            var request = RequestHelper.ToGetRequestMessageAsync(target).Result;

            // Then
            request.RequestUri.ToString().TrimEnd('/')
                .Should().BeEquivalentTo(target.Url.TrimEnd('/'));
        }

        [Test]
        public void Request_Has_Correct_Schema_And_Uri_When_Target_Uri_Is_Fully_Qualified_Http_And_UseHttps_Is_True()
        {
            // Given
            var target = Target.FromUri("http://domain.example.com");

            // Act
            var request = RequestHelper.ToGetRequestMessageAsync(target).Result;

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
            var scanRequest = new ScanRequest("domain.example.com");
            var target = Target.FromUri("/Test/Endpoint");

            // Act
            var request = RequestHelper.ToGetRequestMessageAsync(target, scanRequest.Host).Result;

            // Then
            request.RequestUri.ToString().TrimEnd('/')
                .Should().EndWithEquivalent($"{scanRequest.Host}/{target.Url.Trim('/')}");
        }

        [Test]
        public void Request_Has_Https_Schema_And_Uri_When_Target_Uri_Is_Relative_And_UseHttps_Is_True()
        {
            // Given
            var target = Target.FromUri("/Test/Endpoint");
            var scanRequest = new ScanRequest(target, "domain.example.com");

            // Act
            var request = RequestHelper.ToGetRequestMessageAsync(target, scanRequest.Host).Result;

            // Then
            request.RequestUri.ToString().TrimEnd('/')
                .Should().BeEquivalentTo($"https://{scanRequest.Host}/{target.Url.Trim('/')}");
        }

        [Test]
        public void Request_Has_Http_Schema_And_Uri_When_Target_Uri_Is_Relative_And_UseHttps_Is_False()
        {
            // Given
            var target = Target.FromUri("/Test/Endpoint", false);
            var scanRequest = new ScanRequest(target, "domain.example.com");

            // Act
            var request = RequestHelper.ToGetRequestMessageAsync(target, scanRequest.Host).Result;

            // Then
            request.RequestUri.ToString().TrimEnd('/')
                .Should().BeEquivalentTo($"http://{scanRequest.Host}/{target.Url.Trim('/')}");
        }

        [Test]
        public void Should_Throw_Argument_Exception_When_Relative_Url_And_No_Host()
        {
            // Given
            var target = Target.FromUri("/Test/Endpoint", false);

            // Act
            Action act = () => RequestHelper.ToGetRequestMessage(target, null);

            // Then
            act.Should().ThrowExactly<AggregateException>()
                .WithInnerException<ArgumentNullException>()
                .And.ParamName.Should().Be("host");
        }
    }
}
