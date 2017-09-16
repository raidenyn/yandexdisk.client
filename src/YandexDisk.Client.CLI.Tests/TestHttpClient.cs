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

        private readonly Func<HttpRequestMessage, ResponseContent> _send;
        private readonly HttpStatusCode _httpStatusCode;

        public TestHttpClient(
            Func<HttpRequestMessage, ResponseContent> send,
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
                    Content = GetHttpContent(result)
                };
            });
        }

        private HttpContent GetHttpContent(ResponseContent content)
        {
            if (!String.IsNullOrEmpty(content.Text)) {
                return new StringContent(content.Text, Encoding.UTF8, "text/json");
            }

            if (content.Bites != null)
            {
                return new ByteArrayContent(content.Bites);
            }

            return new StringContent(String.Empty, Encoding.UTF8, "text/json");
        }

        public void Dispose() { }
    }

    internal struct ResponseContent {
        public string Text { get; set; }

        public byte[] Bites { get; set; }
    }
}
