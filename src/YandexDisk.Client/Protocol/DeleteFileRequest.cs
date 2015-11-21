using JetBrains.Annotations;

namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Request of deleting file on Disk
    /// </summary>
    [PublicAPI]
    public class DeleteFileRequest
    {
        /// <summary>
        /// Путь к новому положению ресурса. Например, %2Fbar%2Fphoto.png.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Признак безвозвратного удаления. Поддерживаемые значения:
        /// false — удаляемый файл или папка перемещаются в Корзину(используется по умолчанию).
        /// true — файл или папка удаляются без помещения в Корзину.
        /// </summary>
        public bool Permanently { get; set; }
    }
}
