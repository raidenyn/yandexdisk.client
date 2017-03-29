namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Запрос метаинформации
    /// </summary>
    public class FilesResourceRequest
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

        /// <summary>
        /// Количество вложенных ресурсов с начала списка, которые следует опустить в ответе (например, для постраничного вывода).
        /// Допустим, папка /foo содержит три файла.Если запросить метаинформацию о папке с параметром offset= 1 и сортировкой по умолчанию, API Диска вернет только описания второго и третьего файла.
        /// </summary>
        public int? Offset { get; set; }
    }
}