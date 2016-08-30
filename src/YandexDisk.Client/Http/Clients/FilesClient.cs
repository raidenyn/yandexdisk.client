using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients
{
    internal class FilesClient : DiskClientBase, IFilesClient
    {
        internal FilesClient(ApiContext apiContext)
            : base(apiContext)
        { }

        public Task<Link> GetUploadLinkAsync(string path, bool overwrite, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<object, Link>("resources/upload", new { path, overwrite }, cancellationToken);
        }

        public Task UploadAsync(Link link, Stream file, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = new Uri(link.Href);

            var method = new HttpMethod(link.Method);

            var content = new StreamContent(file);

            var requestMessage = new HttpRequestMessage(method, url) { Content = content };

            return SendAsync(requestMessage, cancellationToken);
        }

        public Task<Link> GetDownloadLinkAsync(string path, CancellationToken cancellationToken)
        {
            return GetAsync<object, Link>("resources/download", new { path }, cancellationToken);
        }

        public async Task<Stream> DownloadAsync(Link link, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = new Uri(link.Href);

            var method = new HttpMethod(link.Method);

            var requestMessage = new HttpRequestMessage(method, url);

            HttpResponseMessage responseMessage = await SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            return await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }
    }
}
