using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients
{
    /// <summary>
    /// Files operation client
    /// </summary>
    [PublicAPI]
    public interface IFilesClient
    {
        /// <summary>
        /// Return link for file upload 
        /// </summary>
        /// <param name="path">Path on Disk for uploading file</param>
        /// <param name="overwrite">If file exists it will be overwritten</param>
        /// <param name="cancellationToken"></param>
        [PublicAPI, NotNull]
        Task<Link> GetUploadLinkAsync([NotNull] string path, bool overwrite, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Upload file to Disk on link recivied by <see cref="GetUploadLinkAsync"/>
        /// </summary>
        [PublicAPI, NotNull]
        Task UploadAsync([NotNull] Link link, [NotNull] Stream file, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Return link for file download 
        /// </summary>
        /// <param name="path">Path to downloading fileon Disk</param>
        /// <param name="cancellationToken"></param>
        [PublicAPI, NotNull]
        Task<Link> GetDownloadLinkAsync([NotNull] string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Download file from Disk on link recivied by <see cref="GetDownloadLinkAsync"/>
        /// </summary>
        [PublicAPI, NotNull]
        Task<Stream> DownloadAsync([NotNull] Link link, CancellationToken cancellationToken = default(CancellationToken));
    }
}
