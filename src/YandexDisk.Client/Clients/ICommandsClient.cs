using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients
{
    /// <summary>
    /// Existing on Disk file operations
    /// </summary>
    [PublicAPI]
    public interface ICommandsClient
    {
        /// <summary>
        /// Create folder on Disk
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> CreateDictionaryAsync([NotNull] string path, CancellationToken cancellationToken);

        /// <summary>
        /// Copy fileor folder on Disk from one path to another
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> CopyAsync([NotNull] CopyFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Move file or folder on Disk from one path to another
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> MoveAsync([NotNull] MoveFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Delete file or folder on Disk
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Link> DeleteAsync([NotNull] DeleteFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Return status of operation
        /// </summary>
        [PublicAPI, ItemNotNull]
        Task<Operation> GetOperationStatus([NotNull] Link link, CancellationToken cancellationToken);
    }


    /// <summary>
    /// Extended file commands
    /// </summary>
    [PublicAPI]
    public static class CommandsClientExtensions
    {
        private static async Task WaitOperationAsync([NotNull] this ICommandsClient client, [NotNull] Link operationLink, CancellationToken cancellationToken, int pullPeriod)
        {
            Operation operation;
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(pullPeriod));
                operation = await client.GetOperationStatus(operationLink, cancellationToken);
            } while (operation.Status == OperationStatus.InProgress &&
                     !cancellationToken.IsCancellationRequested);
        }

        /// <summary>
        /// Copy file or folder on Disk from one path to another and wait until operation is done
        /// </summary>
        /// <returns></returns>
        public static async Task CopyAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] CopyFileRequest request, CancellationToken cancellationToken, int pullPeriod = 3)
        {
            var link = await client.CopyAsync(request, cancellationToken);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod);
            }
        }

        /// <summary>
        /// Move file or folder on Disk from one path to another and wait until operation is done
        /// </summary>
        /// <returns></returns>
        public static async Task MoveAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] MoveFileRequest request, CancellationToken cancellationToken, int pullPeriod = 3)
        {
            var link = await client.MoveAsync(request, cancellationToken);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod);
            }
        }

        /// <summary>
        /// Delete file or folder on Disk and wait until operation is done
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] DeleteFileRequest request, CancellationToken cancellationToken, int pullPeriod = 3)
        {
            var link = await client.DeleteAsync(request, cancellationToken);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod);
            }
        }
    }
}
