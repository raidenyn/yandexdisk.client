using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients
{
    /// <summary>
    /// Files and folder metadata client
    /// </summary>
    public interface IMetaInfoClient
    {
        /// <summary>
        /// Returns information about disk
        /// </summary>
        Task<Disk> GetDiskInfoAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Return files or folder metadata
        /// </summary>
        Task<Resource> GetInfoAsync([NotNull] ResourceRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Return files or folder metadata in the trash
        /// </summary>
        Task<Resource> GetTrashInfoAsync([NotNull] ResourceRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Flat file list on Disk
        /// </summary>
        /// <returns></returns>
        Task<FilesResourceList> GetFilesInfoAsync(FilesResourceRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Last uploaded file list on Disk
        /// </summary>
        Task<LastUploadedResourceList> GetLastUploadedInfoAsync([NotNull] LastUploadedResourceRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Append custom properties to resource
        /// </summary>
        Task<Resource> AppendCustomProperties([NotNull] string path, [NotNull] IDictionary<string, string> properties, CancellationToken cancellationToken);
    }
}
