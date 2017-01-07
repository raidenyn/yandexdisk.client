using System.Collections.Generic;

namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Плоский список всех файлов на Диске в алфавитном порядке.
    /// </summary>
    public class FilesResourceList : ProtocolObjectResponse
    {
        /// <summary>
        /// Массив ресурсов (Resource), содержащихся в папке.
        /// </summary>
        public List<Resource> Items { get; set; }

        /// <summary>
        /// Максимальное количество элементов в массиве items, заданное в запросе.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Смещение начала списка от первого ресурса в папке.
        /// </summary>
        public int Offset { get; set; }
    }
}