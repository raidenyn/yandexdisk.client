using System.Net;
using YandexDisk.Client.Http;
using YandexDisk.Client.Tests;
using YandexDisk.Client.Tests.Polyfils;

namespace YandexDisk.Client.CLI.Tests
{
    public class TestYandexDiskCliProgram: YandexDiskCliProgram
    {
        private readonly IHttpClient _httpClient;

        public TestYandexDiskCliProgram(IHttpClient httpClient) {
            _httpClient = httpClient;
        }

        protected override DiskHttpApi CreateDiskClient(OptionsBase options)
        {
            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: _httpClient);

            return diskClient;
        }
    }
}
