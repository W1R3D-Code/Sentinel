using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Sentinel.Domain;
using Sentinel.Domain.Models.Authentication;
using Sentinel.Domain.Models.Scan;

namespace Sentinel.Test.Request
{
    public class AuthenticationTests : TestBase
	{
        [Test]
        public void Basic_Auth_Head_Should_Be_Added_To_Request_When_Target_Has_Basic_Auth_Configured()
        {
            // Given
            var username = "testUser";
            var password = "testPassword";
            var target = Target.FromUri("https://domain.example.com")
                .WithAuthentication(new BasicAuthentication(username, password));

            // Act
            var request = RequestHelper.ToGetRequestMessageAsync(target).Result;

            var authHeaderPresent =
                request.Headers.TryGetValues(HeaderNames.Authorization, out var authHeader);

            // Then
            authHeaderPresent.Should().BeTrue();
            var header = authHeader.ToList();

            header.Should().NotBeEmpty();
            header.FirstOrDefault().Should().NotBeNullOrEmpty();
            header.FirstOrDefault().Should().StartWithEquivalent("Basic");

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));

            header.FirstOrDefault().Should().BeEquivalentTo($"Basic {encoded}");
        }

        [Test]
        public void Forms_Auth_Should_Make_Auth_Requests_When_Target_Has_Forms_Auth_Configured()
        {
            // Given
            var username = "testUser";
            var password = "testPassword";
            var target = Target.FromUri("https://domain.example.com")
                .WithAuthentication(new FormBasedAuthentication(new Uri("https://domain.example.com/Login"), username, password));

            // Act
            var request = RequestHelper.ToGetRequestMessageAsync(target).Result;

            var authHeaderPresent =
                request.Headers.TryGetValues(ApplicationAuthHeaderName, out var headerValue);

            // Then
            authHeaderPresent.Should().BeTrue();
            var header = headerValue.ToList();

            header.Should().NotBeEmpty();
            header.FirstOrDefault().Should().NotBeNullOrEmpty();
            header.FirstOrDefault().Should().BeEquivalentTo(ApplicationAuthHeaderValue);
        }

        [Test]
        public void Forms_Auth_Should_Throw_Exception_When_Invalid_AntiCsrf_Token_Name()
        {
            // Given
            var username = "testUser";
            var password = "testPassword";
            var target = Target.FromUri("https://domain.example.com")
                .WithAuthentication(new FormBasedAuthentication(new Uri("https://domain.example.com/Login"), username, password)
                {
                    AntiCsrfTokenName = "NotARealTokenName",
                    RequestEncoding = RequestEncoding.FormUrlEncoding
                });

            // Act
            Action act = () => RequestHelper.ToGetRequestMessage(target);

            // Then
            act
                .Should()
                .ThrowExactly<AggregateException>()
                .WithInnerException<Exception>()
                .WithMessage("Can't find Anti-CSRF Token Header");
        }
    }
}
