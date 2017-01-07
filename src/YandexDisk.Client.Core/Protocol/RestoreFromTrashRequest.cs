using JetBrains.Annotations;

namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Request of coping file on Disk
    /// </summary>
    [PublicAPI]
    public class RestoreFromTrashRequest
    {
        /// <summary>
        /// Путь к создаваемой копии ресурса. Например, %2Fbar%2Fphoto.png.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Новое имя восстанавливаемого ресурса. Например, selfie.png.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Признак перезаписи. Учитывается, если ресурс восстанавливается в папку, в которой уже есть ресурс с таким именем.
        /// Допустимые значения:
        /// true — удалять файлы с совпадающими именами и записывать восстанавливаемые файлы.
        /// false — используется по умолчанию.Указывает не перезаписывать файлы и отменить восстановление.
        /// </summary>
        public bool Overwrite { get; set; }
    }
}
