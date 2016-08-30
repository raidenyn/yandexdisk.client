using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients
{
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
                operation = await client.GetOperationStatus(operationLink, cancellationToken).ConfigureAwait(false);
            } while (operation.Status == OperationStatus.InProgress &&
                     !cancellationToken.IsCancellationRequested);
        }

        /// <summary>
        /// Copy file or folder on Disk from one path to another and wait until operation is done
        /// </summary>
        /// <returns></returns>
        public static async Task CopyAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] CopyFileRequest request, CancellationToken cancellationToken = default(CancellationToken), int pullPeriod = 3)
        {
            var link = await client.CopyAsync(request, cancellationToken).ConfigureAwait(false);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Move file or folder on Disk from one path to another and wait until operation is done
        /// </summary>
        /// <returns></returns>
        public static async Task MoveAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] MoveFileRequest request, CancellationToken cancellationToken = default(CancellationToken), int pullPeriod = 3)
        {
            var link = await client.MoveAsync(request, cancellationToken).ConfigureAwait(false);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Delete file or folder on Disk and wait until operation is done
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] DeleteFileRequest request, CancellationToken cancellationToken = default(CancellationToken), int pullPeriod = 3)
        {
            var link = await client.DeleteAsync(request, cancellationToken).ConfigureAwait(false);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Empty trash
        /// </summary>
        /// <returns></returns>
        public static async Task EmptyTrashAndWaitAsyncAsync([NotNull] this ICommandsClient client, [NotNull] string path, CancellationToken cancellationToken = default(CancellationToken), int pullPeriod = 3)
        {
            var link = await client.EmptyTrashAsync(path, cancellationToken).ConfigureAwait(false);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Restore files from trash
        /// </summary>
        /// <returns></returns>
        public static async Task RestoreFromTrashAndWaitAsyncAsync([NotNull] this ICommandsClient client, [NotNull] RestoreFromTrashRequest request, CancellationToken cancellationToken = default(CancellationToken), int pullPeriod = 3)
        {
            var link = await client.RestoreFromTrashAsync(request, cancellationToken).ConfigureAwait(false);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
            }
        }
    }
}
