using System;
using System.Net;
using JetBrains.Annotations;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client
{
    /// <summary>
    /// Exception provide any exceptions risen on yandex server 
    /// </summary>
    [PublicAPI]
    [Serializable]
    public class YandexApiException : Exception
    {
        /// <summary>
        /// Http status code from Yandex server
        /// </summary>
        [PublicAPI]
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Reason phrase from Yandex server
        /// </summary>
        [PublicAPI, CanBeNull]
        public string ReasonPhrase { get; }

        /// <summary>
        /// Error description from Yandex
        /// </summary>
        [PublicAPI, CanBeNull]
        public ErrorDescription Error { get; }

        internal YandexApiException(HttpStatusCode statusCode, string reasonPhrase, ErrorDescription error)
            : base(error?.Description ?? reasonPhrase)
        {
            Error = error;
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
        }

        /// <summary>
        /// Format current exception to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("StatusCode: {0}, {1}. {2}" + Environment.NewLine + "{3}", StatusCode, ReasonPhrase, Message, StackTrace);
        }
    }

    /// <summary>
    /// Exception provide authorization errors on yandex server 
    /// </summary>
    [PublicAPI]
    [Serializable]
    public class NotAuthorizedException : YandexApiException
    {
        internal NotAuthorizedException(string reasonPhrase, ErrorDescription error)
            : base(HttpStatusCode.Unauthorized, reasonPhrase, error)
        { }
    }
}
