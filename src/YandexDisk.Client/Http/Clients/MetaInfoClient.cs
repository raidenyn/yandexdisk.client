using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients
{
    internal class MetaInfoClient : DiskClientBase, IMetaInfoClient
    {
        internal MetaInfoClient(ApiContext apiContext)
            : base(apiContext)
        { }

        public Task<Disk> GetDiskInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<object, Disk>("", /*params*/ null, cancellationToken);
        }

        public Task<Resource> GetInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<ResourceRequest, Resource>("resources", request, cancellationToken);
        }

        public Task<Resource> GetTrashInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<ResourceRequest, Resource>("trash/resources", request, cancellationToken);
        }

        public Task<FilesResourceList> GetFilesInfoAsync(FilesResourceRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<FilesResourceRequest, FilesResourceList>("resources/files", request, cancellationToken);
        }

        public Task<LastUploadedResourceList> GetLastUploadedInfoAsync(LastUploadedResourceRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<LastUploadedResourceRequest, LastUploadedResourceList>("resources/last-uploaded", request, cancellationToken);
        }

        public Task<Resource> AppendCustomProperties(string path, IDictionary<string, string> customProperties, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PatchAsync<object, object, Resource>("resources", new { path }, new { customProperties }, cancellationToken);
        }
    }
}
