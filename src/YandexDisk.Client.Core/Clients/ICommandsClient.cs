using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients
{
    /// <summary>
    /// Disk file operations
    /// </summary>
    [PublicAPI]
    public interface ICommandsClient
    {
        /// <summary>
        /// Create folder on Disk
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> CreateDictionaryAsync([NotNull] string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy fileor folder on Disk from one path to another
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> CopyAsync([NotNull] CopyFileRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Move file or folder on Disk from one path to another
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> MoveAsync([NotNull] MoveFileRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete file or folder on Disk
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> DeleteAsync([NotNull] DeleteFileRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete files in trash
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> EmptyTrashAsync([NotNull] string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete files in trash
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> RestoreFromTrashAsync([NotNull] RestoreFromTrashRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Return status of operation
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Operation> GetOperationStatus([NotNull] Link link, CancellationToken cancellationToken = default(CancellationToken));
    }
}
