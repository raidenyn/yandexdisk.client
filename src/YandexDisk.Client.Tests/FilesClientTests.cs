using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Tests
{
    public class FilesClientTests
    {
        [Test]
        public async Task GetUploadLinkTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + "resources/upload?path=/&overwrite=true", 
                httpStatusCode: HttpStatusCode.OK,
                result:  @"
{
  ""href"": ""https://uploader1d.dst.yandex.net:443/upload-target/..."",
  ""method"": ""PUT"",
  ""templated"": false
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl, 
                                             TestHttpClient.ApiKey, 
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Link result = await diskClient.Files.GetUploadLinkAsync("/", true, CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.AreEqual("https://uploader1d.dst.yandex.net:443/upload-target/...", result.Href);
            Assert.AreEqual("PUT", result.Method);
            Assert.AreEqual(false, result.Templated);
        }

        [Test]
        public async Task UploadTest()
        {
            var link = new Link
            {
                Href = "https://uploader1d.dst.yandex.net/upload-target/",
                Method = "PUT",
                Templated = false
            };

            var httpClientTest = new TestHttpClient(
                methodName: link.Method, 
                url: link.Href, 
                httpStatusCode: HttpStatusCode.Created, 
                result: @"");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            await diskClient.Files.UploadAsync(link, new MemoryStream(), CancellationToken.None).ConfigureAwait(false);
        }

        [Test]
        public async Task GetDownloadLinkTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + "resources/download?path=/file.txt", 
                httpStatusCode: HttpStatusCode.OK,
                result: @"
{
  ""href"": ""https://downloader.dst.yandex.ru/disk/..."",
  ""method"": ""GET"",
  ""templated"": false
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Link result = await diskClient.Files.GetDownloadLinkAsync("/file.txt", CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.AreEqual("https://downloader.dst.yandex.ru/disk/...", result.Href);
            Assert.AreEqual("GET", result.Method);
            Assert.AreEqual(false, result.Templated);
        }

        [Test]
        public async Task DownloadTest()
        {
            var link = new Link
            {
                Href = "https://downloader.dst.yandex.ru/disk/",
                Method = "GET",
                Templated = false
            };

            var httpClientTest = new TestHttpClient(
                methodName: link.Method, 
                url: link.Href, 
                httpStatusCode: HttpStatusCode.Created, 
                result: @"Test file content");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Stream stream = await diskClient.Files.DownloadAsync(link, CancellationToken.None).ConfigureAwait(false);

            var content = new StreamReader(stream).ReadToEnd();

            Assert.AreEqual(@"Test file content", content);
        }
    }
}
