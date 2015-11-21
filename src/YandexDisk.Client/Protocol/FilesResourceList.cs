using System.Collections.Generic;

namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Список последних добавленных на Диск файлов, отсортированных по дате загрузки (от поздних к ранним).
    /// </summary>
    public class LastUploadedResourceList
    {
        /// <summary>
        /// Массив ресурсов (Resource), содержащихся в папке.
        /// </summary>
        public List<Resource> Items { get; set; }

        /// <summary>
        /// Максимальное количество элементов в массиве items, заданное в запросе.
        /// </summary>
        public int Limit { get; set; }
    }
}