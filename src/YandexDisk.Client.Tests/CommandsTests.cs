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

        [Test]
        public async Task CopyTest()
        {
            var httpClientTest = new TestHttpClient("POST", TestHttpClient.BaseUrl + "resources/copy?from=/foo&path=/baz&overwrite=false", HttpStatusCode.Accepted, @"
{
  ""href"": ""https://cloud-api.yandex.net/v1/disk/operations?id=33ca7d03ab21ct41b4a40182e78d828a3f8b72cdb5f4c0e94cc4b1449a63a2fe"",
  ""method"": ""GET"",
  ""templated"": false
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Link result = await diskClient.Commands.CopyAsync(new CopyFileRequest
            {
                From = "/foo",
                Path = "/baz"
            }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual("https://cloud-api.yandex.net/v1/disk/operations?id=33ca7d03ab21ct41b4a40182e78d828a3f8b72cdb5f4c0e94cc4b1449a63a2fe", result.Href);
            Assert.AreEqual("GET", result.Method);
            Assert.AreEqual(false, result.Templated);
        }

        [Test]
        public async Task MoveTest()
        {
            var httpClientTest = new TestHttpClient("POST", TestHttpClient.BaseUrl + "resources/move?from=/foo&path=/baz&overwrite=true", HttpStatusCode.Accepted, @"
{
  ""href"": ""https://cloud-api.yandex.net/v1/disk/operations?id=33ca7d03ab21ct41b4a40182e78d828a3f8b72cdb5f4c0e94cc4b1449a63a2fe"",
  ""method"": ""GET"",
  ""templated"": false
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Link result = await diskClient.Commands.MoveAsync(new MoveFileRequest
            {
                From = "/foo",
                Path = "/baz",
                Overwrite = true
            }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual("https://cloud-api.yandex.net/v1/disk/operations?id=33ca7d03ab21ct41b4a40182e78d828a3f8b72cdb5f4c0e94cc4b1449a63a2fe", result.Href);
            Assert.AreEqual("GET", result.Method);
            Assert.AreEqual(false, result.Templated);
        }

        [Test]
        public async Task DeleteTest()
        {
            var httpClientTest = new TestHttpClient("DELETE", TestHttpClient.BaseUrl + "resources?path=/foo&permanently=false", HttpStatusCode.Accepted, @"
{
  ""href"": ""https://cloud-api.yandex.net/v1/disk/operations?id=d80c269ce4eb16c0207f0a15t4a31415313452f9e950cd9576f36b1146ee0e42"",
  ""method"": ""GET"",
  ""templated"": false
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Link result = await diskClient.Commands.DeleteAsync(new DeleteFileRequest
            {
                Path = "/foo",
            }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual("https://cloud-api.yandex.net/v1/disk/operations?id=d80c269ce4eb16c0207f0a15t4a31415313452f9e950cd9576f36b1146ee0e42", result.Href);
            Assert.AreEqual("GET", result.Method);
            Assert.AreEqual(false, result.Templated);
        }

        [Test]
        public async Task EmptyTrashTest()
        {
            var httpClientTest = new TestHttpClient("DELETE", TestHttpClient.BaseUrl + "trash/resources?path=/foo", HttpStatusCode.Accepted, @"
{
  ""href"": ""https://cloud-api.yandex.net/v1/disk/operations?id=d80c269ce4eb16c0207f0a15t4a31415313452f9e950cd9576f36b1146ee0e42"",
  ""method"": ""GET"",
  ""templated"": false
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Link result = await diskClient.Commands.EmptyTrashAsync(path: "/foo", cancellationToken: CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual("https://cloud-api.yandex.net/v1/disk/operations?id=d80c269ce4eb16c0207f0a15t4a31415313452f9e950cd9576f36b1146ee0e42", result.Href);
            Assert.AreEqual("GET", result.Method);
            Assert.AreEqual(false, result.Templated);
        }

        [Test]
        public async Task RestoreFromTrashTest()
        {
            var httpClientTest = new TestHttpClient("PUT", TestHttpClient.BaseUrl + "trash/resources?path=/foo&name=baz&overwrite=false", HttpStatusCode.OK, @"
{
  ""href"": ""https://cloud-api.yandex.net/v1/disk/resources?path=disk%3A%2Fbar%2Fselfie.png"",
  ""method"": ""GET"",
  ""templated"": false
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Link result = await diskClient.Commands.RestoreFromTrashAsync(new RestoreFromTrashRequest
            {
                Path = "/foo",
                Name = "baz"
            }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual("https://cloud-api.yandex.net/v1/disk/resources?path=disk%3A%2Fbar%2Fselfie.png", result.Href);
            Assert.AreEqual("GET", result.Method);
            Assert.AreEqual(false, result.Templated);
        }

        [Test]
        public async Task GetOperationStatusTest()
        {
            var httpClientTest = new TestHttpClient("GET", TestHttpClient.BaseUrl + "operations/d80c269ce4eb16c0207f0a15t4a31415313452f9e950cd9576f36b1146ee0e42", HttpStatusCode.OK, @"
{
  ""status"":""success""
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Operation result = await diskClient.Commands.GetOperationStatus(new Link
            {
                Href = TestHttpClient.BaseUrl + "operations/d80c269ce4eb16c0207f0a15t4a31415313452f9e950cd9576f36b1146ee0e42",
                Method = "GET"
            }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual(OperationStatus.Success, result.Status);
        }
    }
}
