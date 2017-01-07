using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients
{
    /// <summary>
    /// Extended helpers from uploading and downloading files
    /// </summary>
    public static class FilesClientExtension
    {
        /// <summary>
        /// Just upload stream data to Yandex Disk
        /// </summary>
        [PublicAPI]
        public static async Task UploadFileAsync([NotNull] this IFilesClient client, [NotNull] string path, bool overwrite, [NotNull] Stream file, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            Link link = await client.GetUploadLinkAsync(path, overwrite, cancellationToken).ConfigureAwait(false);

            await client.UploadAsync(link, file, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Just upload file from local disk to Yandex Disk
        /// </summary>
        [PublicAPI]
        public static async Task UploadFileAsync([NotNull] this IFilesClient client, [NotNull] string path, bool overwrite, [NotNull] string localFile, CancellationToken cancellationToken)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (String.IsNullOrWhiteSpace(localFile))
            {
                throw new ArgumentNullException(nameof(localFile));
            }

            Link link = await client.GetUploadLinkAsync(path, overwrite, cancellationToken).ConfigureAwait(false);

            using (var file = new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                await client.UploadAsync(link, file, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get downloaded file from Yandex Disk as stream
        /// </summary>
        [PublicAPI]
        public static async Task<Stream> DownloadFileAsync([NotNull] this IFilesClient client, [NotNull] string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Link link = await client.GetDownloadLinkAsync(path, cancellationToken).ConfigureAwait(false);

            return await client.DownloadAsync(link, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloaded data from Yandex Disk to local file
        /// </summary>
        [PublicAPI]
        public static async Task DownloadFileAsync([NotNull] this IFilesClient client, [NotNull] string path, [NotNull] string localFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (String.IsNullOrWhiteSpace(localFile))
            {
                throw new ArgumentNullException(nameof(localFile));
            }

            Stream data = await DownloadFileAsync(client, path, cancellationToken).ConfigureAwait(false);

            using (var file = new FileStream(localFile, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
            {
                await data.CopyToAsync(file, bufferSize: 81920/*keep default*/, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
