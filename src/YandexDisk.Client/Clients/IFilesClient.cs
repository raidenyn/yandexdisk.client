using System;
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
        Task<Link> GetUploadLinkAsync([NotNull] string path, bool overwrite, CancellationToken cancellationToken);

        /// <summary>
        /// Upload file to Disk on link recivied by <see cref="GetUploadLinkAsync"/>
        /// </summary>
        [PublicAPI, NotNull]
        Task UploadAsync([NotNull] Link link, [NotNull] Stream file, CancellationToken cancellationToken);

        /// <summary>
        /// Return link for file download 
        /// </summary>
        /// <param name="path">Path to downloading fileon Disk</param>
        /// <param name="cancellationToken"></param>
        [PublicAPI, NotNull]
        Task<Link> GetDownloadLinkAsync([NotNull] string path, CancellationToken cancellationToken);

        /// <summary>
        /// Download file from Disk on link recivied by <see cref="GetDownloadLinkAsync"/>
        /// </summary>
        [PublicAPI, NotNull]
        Task<Stream> DownloadAsync([NotNull] Link link, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Extended helpers from uploading and downloading files
    /// </summary>
    public static class FilesClientExtension
    {
        /// <summary>
        /// Just upload stream data to Yandex Disk
        /// </summary>
        [PublicAPI]
        public static async Task UploadFileAsync([NotNull] this IFilesClient client, [NotNull] string path, bool overwrite, [NotNull] Stream file, CancellationToken cancellationToken)
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

            Link link = await client.GetUploadLinkAsync(path, overwrite, cancellationToken);

            await client.UploadAsync(link, file, cancellationToken);
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

            Link link = await client.GetUploadLinkAsync(path, overwrite, cancellationToken);

            using (var file = new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                await client.UploadAsync(link, file, cancellationToken);
            }
        }

        /// <summary>
        /// Get downloaded file from Yandex Disk as stream
        /// </summary>
        [PublicAPI]
        public static async Task<Stream> DownloadFileAsync([NotNull] this IFilesClient client, [NotNull] string path, CancellationToken cancellationToken)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Link link = await client.GetDownloadLinkAsync(path, cancellationToken);

            return await client.DownloadAsync(link, cancellationToken);
        }

        /// <summary>
        /// Downloaded data from Yandex Disk to local file
        /// </summary>
        [PublicAPI]
        public static async Task DownloadFileAsync([NotNull] this IFilesClient client, [NotNull] string path, [NotNull] string localFile, CancellationToken cancellationToken)
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

            Stream data = await DownloadFileAsync(client, path, cancellationToken);

            using (var file = new FileStream(localFile, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
            {
                await data.CopyToAsync(file, bufferSize: 81920/*keep default*/, cancellationToken: cancellationToken);
            }
        }


        #region Methods with CancelationToken.None

        /// <summary>
        /// Just upload stream data to Yandex Disk
        /// </summary>
        public static Task UploadFileAsync(this IFilesClient client, string path, bool overwrite, Stream file)
        {
            return UploadFileAsync(client, path, overwrite, file, CancellationToken.None);
        }

        /// <summary>
        /// Just upload file from local disk to Yandex Disk
        /// </summary>
        public static Task UploadFileAsync(this IFilesClient client, string path, bool overwrite, string localFile)
        {
            return UploadFileAsync(client, path, overwrite, localFile, CancellationToken.None);
        }

        /// <summary>
        /// Get downloaded file from Yandex Disk as stream
        /// </summary>
        public static Task<Stream> DownloadFileAsync(this IFilesClient client, string path)
        {
            return DownloadFileAsync(client, path, CancellationToken.None);
        }

        /// <summary>
        /// Downloaded data from Yandex Disk to local file
        /// </summary>
        public static Task DownloadFileAsync(this IFilesClient client, string path, string localFile)
        {
            return DownloadFileAsync(client, path, localFile, CancellationToken.None);
        }

        #endregion
    }
}
