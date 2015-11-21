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
        Task<FilesResourceList> GetFilesInfoAsync([NotNull] FilesResourceRequest request, CancellationToken cancellationToken);
    }
}
