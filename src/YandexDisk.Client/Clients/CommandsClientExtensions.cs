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
        /// <summary>
        /// Default pull period for waiting operation.
        /// </summary>
        public static TimeSpan DefaultPullPeriod = TimeSpan.FromSeconds(3);

        private static async Task WaitOperationAsync([NotNull] this ICommandsClient client, [NotNull] Link operationLink, CancellationToken cancellationToken, TimeSpan? pullPeriod)
        {
            if (pullPeriod == null)
            {
                pullPeriod = DefaultPullPeriod;
            }

            Operation operation;
            do
            {
                Thread.Sleep(pullPeriod.Value);
                operation = await client.GetOperationStatus(operationLink, cancellationToken).ConfigureAwait(false);
            } while (operation.Status == OperationStatus.InProgress &&
                     !cancellationToken.IsCancellationRequested);
        }

        /// <summary>
        /// Copy file or folder on Disk from one path to another and wait until operation is done
        /// </summary>
        public static async Task CopyAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] CopyFileRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
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
        public static async Task MoveAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] MoveFileRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
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
        public static async Task DeleteAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] DeleteFileRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
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
        public static async Task EmptyTrashAndWaitAsyncAsync([NotNull] this ICommandsClient client, [NotNull] string path, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
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
        public static async Task RestoreFromTrashAndWaitAsyncAsync([NotNull] this ICommandsClient client, [NotNull] RestoreFromTrashRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
        {
            var link = await client.RestoreFromTrashAsync(request, cancellationToken).ConfigureAwait(false);

            if (link.HttpStatusCode == HttpStatusCode.Accepted)
            {
                await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
            }
        }


        #region Obsoleted
        /// <summary>
        /// Copy file or folder on Disk from one path to another and wait until operation is done
        /// </summary>
        [Obsolete("Method is obsolete. Please use CopyAndWaitAsync(ICommandsClient, CopyFileRequest, CancellationToken, TimeSpan) instead.")]
        public static Task CopyAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] CopyFileRequest request, CancellationToken cancellationToken, int pullPeriod)
        {
            return CopyAndWaitAsync(client, request, cancellationToken, TimeSpan.FromSeconds(pullPeriod));
        }


        /// <summary>
        /// Move file or folder on Disk from one path to another and wait until operation is done
        /// </summary>
        [Obsolete("Method is obsolete. Please use MoveAndWaitAsync(ICommandsClient, MoveFileRequest, CancellationToken, TimeSpan) instead.")]
        public static Task MoveAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] MoveFileRequest request, CancellationToken cancellationToken, int pullPeriod)
        {
            return MoveAndWaitAsync(client, request, cancellationToken, TimeSpan.FromSeconds(pullPeriod));
        }

        /// <summary>
        /// Delete file or folder on Disk and wait until operation is done
        /// </summary>
        [Obsolete("Method is obsolete. Please use DeleteAndWaitAsync(ICommandsClient, DeleteFileRequest, CancellationToken, TimeSpan) instead.")]
        public static Task DeleteAndWaitAsync([NotNull] this ICommandsClient client, [NotNull] DeleteFileRequest request, CancellationToken cancellationToken, int pullPeriod)
        {
            return DeleteAndWaitAsync(client, request, cancellationToken, TimeSpan.FromSeconds(pullPeriod));
        }

        /// <summary>
        /// Empty trash
        /// </summary>
        [Obsolete("Method is obsolete. Please use EmptyTrashAndWaitAsyncAsync(ICommandsClient, string, CancellationToken, TimeSpan) instead.")]
        public static Task EmptyTrashAndWaitAsyncAsync([NotNull] this ICommandsClient client, [NotNull] string path, CancellationToken cancellationToken, int pullPeriod)
        {
            return EmptyTrashAndWaitAsyncAsync(client, path, cancellationToken, TimeSpan.FromSeconds(pullPeriod));
        }

        /// <summary>
        /// Restore files from trash
        /// </summary>
        [Obsolete("Method is obsolete. Please use RestoreFromTrashAndWaitAsyncAsync(ICommandsClient, RestoreFromTrashRequest, CancellationToken, TimeSpan) instead.")]
        public static Task RestoreFromTrashAndWaitAsyncAsync([NotNull] this ICommandsClient client, [NotNull] RestoreFromTrashRequest request, CancellationToken cancellationToken, int pullPeriod)
        {
            return RestoreFromTrashAndWaitAsyncAsync(client, request, cancellationToken, TimeSpan.FromSeconds(pullPeriod));
        }

        #endregion
    }
}
