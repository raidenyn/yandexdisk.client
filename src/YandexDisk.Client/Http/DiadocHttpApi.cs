using System;
using System.Net.Http;
using System.Net.Http.Headers;
using JetBrains.Annotations;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http.Clients;

namespace YandexDisk.Client.Http
{
    /// <remarks>
    /// This object is thread safe. You should cache it between requests. 
    /// </remarks>
    [PublicAPI]
    public class DiskHttpApi : IDiskApi
    {
        private readonly IHttpClient _httpClient;

        /// <summary>
        /// Default base url to Yandex Disk API
        /// </summary>
        public string BaseUrl { get; } = "https://cloud-api.yandex.net/v1/disk/";

        /// <summary>
        /// Create new instance of DiskHttpApi. Keep one instance for all requests.
        /// </summary>
        /// <param name="oauthKey">
        /// OAuth Key for authorization on API
        /// <see href="https://tech.yandex.ru/disk/api/concepts/quickstart-docpage/"/>
        /// </param>
        /// <param name="logSaver">Instance of custom logger. It noticed on each request-response API operation.</param>
        [PublicAPI]
        public DiskHttpApi([NotNull] string oauthKey, [CanBeNull] ILogSaver logSaver = null)
        {
            var clientHandler = new HttpClientHandler();
     
            var httpClient = new HttpClient(clientHandler, disposeHandler: true);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", oauthKey);
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(AboutInfo.Client.ProductTitle, AboutInfo.Client.Version));
            httpClient.Timeout = TimeSpan.FromHours(24); //For support large file uploading and downloading 

            _httpClient = new RealHttpClientWrapper(httpClient);

            var apiContext = new ApiContext
            {
                HttpClient = _httpClient,
                BaseUrl = new Uri(BaseUrl),
                LogSaver = logSaver
            };

            Files = new FilesClient(apiContext);
            MetaInfo = new MetaInfoClient(apiContext);
            Commands = new CommandsClient(apiContext);
        }

        /// <summary>
        /// Create new instance of DiskHttpApi. Keep one instance for all requests.
        /// </summary>
        /// <param name="baseUrl">Base url to Yandex Disk API.</param>
        /// <param name="oauthKey">
        /// OAuth Key for authorization on API
        /// <see href="https://tech.yandex.ru/disk/api/concepts/quickstart-docpage/"/>
        /// </param>
        /// <param name="logSaver">Instance of custom logger.</param>
        /// <param name="httpClient"></param>
        public DiskHttpApi([NotNull] string baseUrl, [NotNull] string oauthKey, [CanBeNull] ILogSaver logSaver, [NotNull] IHttpClient httpClient)
        {
            BaseUrl = baseUrl;
            _httpClient = httpClient;

            var apiContext = new ApiContext
            {
                HttpClient = httpClient,
                BaseUrl = new Uri(baseUrl),
                LogSaver = logSaver
            };

            Files = new FilesClient(apiContext);
            MetaInfo = new MetaInfoClient(apiContext);
            Commands = new CommandsClient(apiContext);
        }

        #region Clients

        /// <summary>
        /// Uploading and downloading file operation
        /// </summary>
        [PublicAPI]
        public IFilesClient Files { get; }

        /// <summary>
        /// Getting files and folders metadata  
        /// </summary>
        [PublicAPI]
        public IMetaInfoClient MetaInfo { get; }

        /// <summary>
        /// Manipulating with existing files and folders 
        /// </summary>
        [PublicAPI]
        public ICommandsClient Commands { get; }

        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        [PublicAPI]
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
