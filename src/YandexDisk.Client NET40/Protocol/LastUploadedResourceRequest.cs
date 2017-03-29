namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Запрос метаинформации
    /// </summary>
    public class LastUploadedResourceRequest
    {
        /// <summary>
        /// Тип файлов, которые нужно включить в список. Диск определяет тип каждого файла при загрузке.
        /// </summary>
        /// <see>https://tech.yandex.ru/disk/api/reference/all-files-docpage/</see>
        public MediaType[] MediaType { get; set; }

        /// <summary>
        /// Количество ресурсов, вложенных в папку, описание которых следует вернуть в ответе (например, для постраничного вывода).
        /// Значение по умолчанию — 20.
        /// </summary>
        public int? Limit { get; set; }
    }
}