﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Sentinel.Domain;
using Sentinel.Domain.Extensions;
using Sentinel.Domain.Models.Scan;
using Sentinel.Domain.Models.Authentication;

namespace Sentinel.Test
{
    public class RequestAuthenticationTests
    {
        [Test]
        public void Basic_Auth_Head_Should_Be_Added_To_Request_When_Target_Has_Basic_Auth_Configured()
        {
            var username = "testUser";
            var password = "testPassword";
            var target = new Target {Authentication = new BasicAuthentication(username, password)};

            target.Authentication.PreRequestAction();

            var request = new HttpRequestMessage();
            target.Authentication.AddRequestAuthentication(request);

            var authHeaderPresent =
            request.Headers.TryGetValues(HeaderNames.Authorization, out var authHeader);

            authHeaderPresent.Should().BeTrue();
            var header = authHeader.ToList();

            header.Should().NotBeEmpty();
            header.FirstOrDefault().Should().NotBeNullOrEmpty();
            header.FirstOrDefault().Should().StartWithEquivalent("Basic");

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));

            header.FirstOrDefault().Should().BeEquivalentTo($"Basic {encoded}");
        }
    }
}
