using System;
using JetBrains.Annotations;
using YandexDisk.Client.Clients;

namespace YandexDisk.Client
{
    /// <summary>
    /// Definition of all methods od Yandex Disk API
    /// </summary>
    [PublicAPI]
    public interface IDiskApi : IDisposable
    {
        /// <summary>
        /// Uploading and downloading file operation
        /// </summary>
        [PublicAPI, NotNull]
        IFilesClient Files { get; }

        /// <summary>
        /// Getting files and folders metadata  
        /// </summary>
        [PublicAPI, NotNull]
        IMetaInfoClient MetaInfo { get; }

        /// <summary>
        /// Manipulating with existing files and folders 
        /// </summary>
        [PublicAPI, NotNull]
        ICommandsClient Commands { get; }
    }
}
