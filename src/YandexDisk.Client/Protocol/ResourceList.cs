using System.Collections.Generic;

namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Список ресурсов, содержащихся в папке. Содержит объекты Resource и свойства списка.
    /// </summary>
    public class ResourceList
    {
        /// <summary>
        /// Поле, по которому отсортирован список.
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// Ключ опубликованной папки, в которой содержатся ресурсы из данного списка.
        /// Включается только в ответ на запрос метаинформации о публичной папке.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Массив ресурсов (Resource), содержащихся в папке.
        /// </summary>
        public List<Resource> Items { get; set; }

        /// <summary>
        /// Путь к папке, чье содержимое описывается в данном объекте ResourceList.
        /// Для публичной папки значение атрибута всегда равно «/».
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Максимальное количество элементов в массиве items, заданное в запросе.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Смещение начала списка от первого ресурса в папке.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Общее количество ресурсов в папке.
        /// </summary>
        public int Total { get; set; }
    }
}