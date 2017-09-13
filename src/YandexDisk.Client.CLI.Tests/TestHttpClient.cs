using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using YandexDisk.Client.Http;
using System;

namespace YandexDisk.Client.Tests
{
    internal class TestHttpClient : IHttpClient
    {
        public static readonly string BaseUrl = "http://ya.ru/api/";
        public static readonly string ApiKey = "test-api-key";

        private readonly Func<HttpRequestMessage, string> _send;
        private readonly HttpStatusCode _httpStatusCode;

        public TestHttpClient(
            Func<HttpRequestMessage, string> send,
            HttpStatusCode httpStatusCode = HttpStatusCode.OK
        ) {
            _send = send;
            _httpStatusCode = httpStatusCode;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => {
                var result = _send(request);

                return new HttpResponseMessage(_httpStatusCode)
                {
                    Content = new StringContent(result, Encoding.UTF8, "text/json")
                };
            });
        }

        public void Dispose() { }
    }
}
