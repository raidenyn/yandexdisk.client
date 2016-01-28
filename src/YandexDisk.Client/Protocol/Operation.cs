using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Статус операции. Операции запускаются, когда вы копируете, перемещаете или удаляете непустые папки.
    /// URL для запроса статуса возвращается в ответ на такие запросы.
    /// </summary>
    public class Operation : ProtocolObjectResponse
    {
        /// <summary>
        /// Статус операции
        /// </summary>
        public OperationStatus Status { get; set; }
    }

    /// <summary>
    /// Возможные статусы опреаций
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OperationStatus
    {
        /// <summary>
        /// Операция успешно завершена.
        /// </summary>
        Success,

        /// <summary>
        /// Операцию совершить не удалось, попробуйте повторить изначальный запрос копирования, перемещения или удаления.
        /// </summary>
        Failure,

        /// <summary>
        /// Операция начата, но еще не завершена.
        /// </summary>
        [EnumMember(Value = "in-progress")]
        InProgress
    }
}