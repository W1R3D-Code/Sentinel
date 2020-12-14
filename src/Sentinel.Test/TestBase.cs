using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Sentinel.Scanner.Helpers;

namespace Sentinel.Test
{
    public abstract class TestBase
    {
        protected const string AntiCsrfHeaderName = "__RequestVerificationToken";
        protected const string AntiCsrfHeaderValue = "h9teFdCSYrGHhut8I3fTyu7g5j2fpFPv_xZ9WfSz-74VFfFxLr7EeyAu3ya2EjxhDflxDAeFChni7hFshLb5w-TWrI8SSKFUP2j1wNCH1z41";

        protected const string ApplicationAuthHeaderName = "ASPXAUTH";
        protected const string ApplicationAuthHeaderValue = "5A854A1BE838C1CB2B1AFA64277A6EBAA1F4E7E3B32FB26F73F3ABA9A5F67307ED919C305885CB99321EA56B2DD2D9B7CF67BF8E2764853DFB36B2CC8AF9416B67A2086BF5612058378D67D22C2330412DCBFD7EF297D669F634108E95BB916360AAE8172807725AA4A4F512C3AA947D";

        protected const string ApplicationSessionCookieName = "ASP.NET_SessionId";
        protected const string ApplicationSessionCookieValue = "dce4favbuztb342a4dau3trx";

        protected RequestHelper RequestHelper;

        [OneTimeSetUp]
		public void Setup()
        {
            SetFakeResponse(HttpStatusCode.OK);
        }

		public void SetFakeResponse(HttpStatusCode statusCode, HttpContent content = null)
        {
			var fakeResponse = GetFakeResponse(statusCode, content);
            RequestHelper = new RequestHelper(new MockHttpClientWrapper(fakeResponse), new NullLoggerFactory());
		}

		private HttpResponseMessage GetFakeResponse(HttpStatusCode statusCode, HttpContent content = null)
        {
            var fakeResponse = new HttpResponseMessage(statusCode)
            {
                Content = content ?? new StringContent(
                    @"<form action="""" autocomplete=""off"" class=""form-horizontal"" id=""instanda-cp-login-form"" method=""post"" role=""form"" novalidate=""novalidate"">
	<input name=""__RequestVerificationToken"" type=""hidden"" value=""KktMjNECEz0MO8HVw5IDVIehlUtNGsOhgrEqEFuNpG7S_XskMy-a0rY9A8r0ASuADHI5V85vLiGaSA_Y9LsIbnDRDBZFK50ELMrgf01JeBQ1"">      
	<div class=""form-group"">
		<div class=""col-md-12"">
		</div>
	</div>
	<div class=""form-group"">
		<div class=""col-md-12"">
			<input autocomplete=""off"" autofocus=""autofocus"" class=""form-control"" data-val=""true"" data-val-email=""Please use a valid email address."" data-val-required=""Email address is required"" id=""instanda-cp-user-name"" name=""UserName"" placeholder=""email address"" type=""text"" value="""">
			<span class=""field-validation-valid label label-warning"" data-valmsg-for=""UserName"" data-valmsg-replace=""true""></span>
		</div>
	</div>
	<div class=""form-group"">
		<div class=""col-md-12"">
			<input id=""Password"" name=""Password"" placeholder=""password"" type=""password"">
			<span class=""field-validation-valid label label-warning"" data-valmsg-for=""Password"" data-valmsg-replace=""true""></span>
		</div>
	</div>
	<div class=""form-group"">
		<div class=""col-md-9 "">
			<button type=""submit"" class=""btn btn-primary  instanda-btn"" data-loading-text=""Signing in..."" role=""button"">Sign in</button>
		</div>
	</div>
	<div class=""form-group"" style=""margin-bottom:0"">
		<div class=""col-md-9"" style=""margin-bottom:0"">
			<a href=""#"" style=""margin-right:10px"" id=""instanda-cp-forgot-link"">Forgot your password?</a>
		</div>
	</div>
	<input id=""ReturnUrl"" name=""ReturnUrl"" type=""hidden"" value=""/Public/PreQuoteQuestions?PackageId=13138&amp;pageNumber=1"">
</form>")
            };

            fakeResponse.Headers.Add(AntiCsrfHeaderName, AntiCsrfHeaderValue);
            fakeResponse.Headers.Add(ApplicationSessionCookieName, ApplicationSessionCookieValue);
            fakeResponse.Headers.Add(ApplicationAuthHeaderName, ApplicationAuthHeaderValue);

			// Include Set-Cookie headers since we're using this fake response as a swiss army knife for all the tests
            fakeResponse.Headers.TryAddWithoutValidation("Set-Cookie", $"{AntiCsrfHeaderName}={AntiCsrfHeaderValue}");
            fakeResponse.Headers.TryAddWithoutValidation("Set-Cookie", $"{ApplicationSessionCookieName}={ApplicationSessionCookieValue}");
            fakeResponse.Headers.TryAddWithoutValidation("Set-Cookie", $"{ApplicationAuthHeaderName}={ApplicationAuthHeaderValue}");

			return fakeResponse;
        }
    }
}
