namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Дополнительное описание ошибки
    /// </summary>
    public class ErrorDescription
    {
        /// <summary>
        /// Подробное описание ошибки в помощь разработчику.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Идентификатор ошибки для программной обработки.
        /// </summary>
        public string Error { get; set; }
    }
}