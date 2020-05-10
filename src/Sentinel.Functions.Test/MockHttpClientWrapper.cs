using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sentinel.Scanner;

namespace Sentinel.Functions.Test
{
    public class MockHttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpResponseMessage _responseToSend;

        public MockHttpClientWrapper(HttpResponseMessage responseToSend)
        {
            _responseToSend = responseToSend;
        }

        public async Task<HttpResponseMessage> TrySendRequest(HttpRequestMessage message, CancellationToken cancellationToken, bool disposeClient = false)
        {
            return _responseToSend;
        }
    }
}
