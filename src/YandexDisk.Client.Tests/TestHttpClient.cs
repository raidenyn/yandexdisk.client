using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using YandexDisk.Client.Http;

namespace YandexDisk.Client.Tests
{
    internal class TestHttpClient : IHttpClient
    {
        private readonly string _methodName;
        private readonly string _url;
        private readonly HttpStatusCode _httpStatusCode;
        private readonly string _result;

        public static readonly string BaseUrl = "http://ya.ru/api/";
        public static readonly string ApiKey = "test-api-key";

        public TestHttpClient(string methodName,
                              string url,
                              HttpStatusCode httpStatusCode,
                              string result)
        {
            _methodName = methodName;
            _url = url;
            _httpStatusCode = httpStatusCode;
            _result = result;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Assert.NotNull(request);
            Assert.AreEqual(_methodName, request.Method.Method);
            Assert.AreEqual(_url, request.RequestUri.ToString());

            return Task.FromResult(new HttpResponseMessage(_httpStatusCode)
            {
                Content = new StringContent(_result, Encoding.UTF8, "text/json")
            });
        }

        public void Dispose() { }
    }
}
