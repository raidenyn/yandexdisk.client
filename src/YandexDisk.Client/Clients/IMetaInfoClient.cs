using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients
{
    /// <summary>
    /// Метаинформация о файле или папке
    /// </summary>
    public interface IMetaInfoClient
    {
        /// <summary>
        /// Метаинформация о файле или папке
        /// </summary>
        Task<Resource> GetInfoAsync([NotNull] ResourceRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Метаинформация о файле или папке в корзине
        /// </summary>
        Task<Resource> GetTrashInfoAsync([NotNull] ResourceRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Плоский список всех файлов
        /// </summary>
        Task<FilesResourceList> GetFilesInfoAsync([NotNull] FilesResourceRequest request, CancellationToken cancellationToken);
    }
}
