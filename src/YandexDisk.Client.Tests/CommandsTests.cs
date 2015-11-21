using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Tests
{
    public class CommandsTests
    {
        [Test]
        public async Task CreateDictionaryTest()
        {
            var httpClientTest = new TestHttpClient("PUT", TestHttpClient.BaseUrl + "resources?path=/foo", HttpStatusCode.OK, @"
{
  ""href"": ""https://cloud-api.yandex.net/v1/disk/resources?path=disk%3A%2FMusic"",
  ""method"": ""GET"",
  ""templated"": false
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Link result = await diskClient.Commands.CreateDictionaryAsync("/foo", CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual("https://cloud-api.yandex.net/v1/disk/resources?path=disk%3A%2FMusic", result.Href);
            Assert.AreEqual("GET", result.Method);
            Assert.AreEqual(false, result.Templated);
        }
    }
}
