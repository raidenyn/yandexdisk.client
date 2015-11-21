using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients
{
    /// <summary>
    /// Операции над файлами
    /// </summary>
    [PublicAPI]
    public interface ICommandsClient
    {
        /// <summary>
        /// Создание папки
        /// </summary>
        [PublicAPI, NotNull]
        Task<Link> CreateDictionaryAsync([NotNull] string path, CancellationToken cancellationToken);
    }
}
