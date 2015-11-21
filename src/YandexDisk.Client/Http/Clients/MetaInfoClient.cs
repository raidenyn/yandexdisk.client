using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients
{
    internal class MetaInfoClient : DiadocClientBase, IMetaInfoClient
    {
        internal MetaInfoClient(ApiContext apiContext)
            : base(apiContext)
        { }

        public Task<Resource> GetInfoAsync(ResourceRequest request, CancellationToken cancellationToken)
        {
            return GetAsync<ResourceRequest, Resource>("resources", request, cancellationToken);
        }

        public Task<Resource> GetTrashInfoAsync(ResourceRequest request, CancellationToken cancellationToken)
        {
            return GetAsync<ResourceRequest, Resource>("trash/resources", request, cancellationToken);
        }

        public Task<FilesResourceList> GetFilesInfoAsync(FilesResourceRequest request, CancellationToken cancellationToken)
        {
            return GetAsync<FilesResourceRequest, FilesResourceList>("resources/files", request, cancellationToken);
        }
    }
}
