using System.Net;

namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Base class of protocol object
    /// </summary>
    public class ProtocolObjectResponse
    {
        /// <summary>
        /// Http status code of response from Yandex Disk API
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
