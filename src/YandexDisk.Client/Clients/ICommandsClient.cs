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
        [PublicAPI, NotNull]
        Task<Link> CreateDictionaryAsync([NotNull] string path, CancellationToken cancellationToken);

        /// <summary>
        /// Copy fileor folder on Disk from one path to another
        /// </summary>
        [PublicAPI, NotNull]
        Task<Link> CopyAsync(CopyFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Move file or folder on Disk from one path to another
        /// </summary>
        [PublicAPI, NotNull]
        Task<Link> MoveAsync(MoveFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Delete file or folder on Disk
        /// </summary>
        [PublicAPI, NotNull]
        Task<Link> DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Return status of operation
        /// </summary>
        [PublicAPI, NotNull]
        Task<Operation> GetOperationStatus(Link link, CancellationToken cancellationToken);
    }


    /// <summary>
    /// Extended file commands
    /// </summary>
    [PublicAPI]
    public static class CommandsClientExtensions
    {
        private static async Task WaitOperationAsync(this ICommandsClient client, Link operationLink, CancellationToken cancellationToken, int pullPeriod)
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
        public static async Task CopyAndWaitAsync(this ICommandsClient client, CopyFileRequest request, CancellationToken cancellationToken, int pullPeriod = 3)
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
        public static async Task MoveAndWaitAsync(this ICommandsClient client, MoveFileRequest request, CancellationToken cancellationToken, int pullPeriod = 3)
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
        public static async Task DeleteAndWaitAsync(this ICommandsClient client, DeleteFileRequest request, CancellationToken cancellationToken, int pullPeriod = 3)
        {
            var link = await client.DeleteAsync(request, cancellationToken);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod);
            }
        }
    }
}
