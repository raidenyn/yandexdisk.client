using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YandexDisk.Client.Http.Serialization;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http
{
    internal abstract class DiadocClientBase
    {
        private static readonly QueryParamsSerializer MvcSerializer = new QueryParamsSerializer();

        public readonly MediaTypeFormatter[] DefaultFormatters =
        {
            new JsonMediaTypeFormatter
            {
                SerializerSettings =
                {
                    ContractResolver = new SnakeCasePropertyNamesContractResolver()
                }
            }
        };

        private readonly IHttpClient _httpClient;
        private readonly ILogSaver _logSaver;
        public readonly Uri BaseUrl;

        protected DiadocClientBase([NotNull] ApiContext apiContext)
        {
            if (apiContext == null)
            {
                throw new ArgumentNullException(nameof(apiContext));
            }
            if (apiContext.HttpClient == null)
            {
                throw new ArgumentNullException(nameof(apiContext.HttpClient));
            }
            if (apiContext.BaseUrl == null)
            {
                throw new ArgumentNullException(nameof(apiContext.BaseUrl));
            }

            _httpClient = apiContext.HttpClient;
            _logSaver = apiContext.LogSaver;
            BaseUrl = apiContext.BaseUrl;
        }

        [NotNull]
        protected Uri GetUrl([NotNull] string relativeUrl, [CanBeNull] object request = null)
        {
            if (relativeUrl == null)
            {
                throw new ArgumentNullException(nameof(relativeUrl));
            }

            var uriBuilder = new UriBuilder(BaseUrl);
            uriBuilder.Path += relativeUrl;

            if (request != null)
            {
                uriBuilder.Query = MvcSerializer.Serialize(request);
            }

            return uriBuilder.Uri;
        }

        [CanBeNull]
        protected string BuildPath([CanBeNull] string relativeUrlTemplate, [NotNull] params string[] values)
        {
            if (String.IsNullOrWhiteSpace(relativeUrlTemplate))
            {
                return relativeUrlTemplate;
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            for (var i = 0; i < values.Length; i++)
            {
                values[i] = Uri.EscapeDataString(values[i]);
            }

            // ReSharper disable once CoVariantArrayConversion
            return String.Format(relativeUrlTemplate, values);
        }

        [CanBeNull]
        protected HttpContent GetContent<TRequest>([CanBeNull] TRequest request)
            where TRequest : class
        {
            if (request == null)
            {
                return null;
            }

            if (typeof(TRequest) == typeof(string))
            {
                return new StringContent(request as string);
            }
            if (typeof(TRequest) == typeof(byte[]))
            {
                return new ByteArrayContent(request as byte[]);
            }
            if (typeof(Stream).IsAssignableFrom(typeof(TRequest)))
            {
                return new StreamContent(request as Stream);
            }

            return new ObjectContent<TRequest>(request, DefaultFormatters.First());
        }

        [NotNull]
        protected internal virtual ILogger GetLogger()
        {
            return LoggerFactory.GetLogger(_logSaver);
        }

        [ItemNotNull]
        protected internal async Task<TResponse> SendAsync<TResponse>([NotNull] HttpRequestMessage request, CancellationToken cancellationToken)
            where TResponse : class
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            HttpResponseMessage response = await SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response);
        }

        [ItemNotNull]
        protected internal virtual async Task<HttpResponseMessage> SendAsync([NotNull] HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (ILogger logger = GetLogger())
            {
                await logger.SetRequestAsync(request);

                try
                {
                    HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                    await logger.SetResponseAsync(response);

                    await EnsureSuccessStatusCode(response);
                    
                    logger.EndWithSuccess();

                    return response;
                }
                catch (Exception e)
                {
                    logger.EndWithError(e);

                    throw;
                }
            }
        }

        [ItemNotNull]
        protected internal virtual async Task<TResponse> ReadResponse<TResponse>([NotNull] HttpResponseMessage responseMessage)
            where TResponse : class
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            if (responseMessage.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }
            if (typeof(TResponse) == typeof(string))
            {
                return await responseMessage.Content.ReadAsStringAsync() as TResponse;
            }
            if (typeof(TResponse) == typeof(byte[]))
            {
                return await responseMessage.Content.ReadAsByteArrayAsync() as TResponse;
            }
            if (typeof(Stream).IsAssignableFrom(typeof(TResponse)))
            {
                return await responseMessage.Content.ReadAsStreamAsync() as TResponse;
            }

            return await responseMessage.Content.ReadAsAsync<TResponse>(DefaultFormatters);
        }

        [ItemNotNull]
        protected internal Task<TResponse> GetAsync<TParams, TResponse>([NotNull] string relativeUrl, [CanBeNull] TParams parameters, CancellationToken cancellationToken)
            where TParams : class
            where TResponse : class
        {
            if (relativeUrl == null)
            {
                throw new ArgumentNullException(nameof(relativeUrl));
            }

            Uri url = GetUrl(relativeUrl, parameters);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            return SendAsync<TResponse>(requestMessage, cancellationToken);
        }

        [ItemNotNull]
        protected internal Task<TResponse> RequestAsync<TRequest, TParams, TResponse>([NotNull] string relativeUrl, [CanBeNull] TParams parameters, [CanBeNull] TRequest request, [NotNull] HttpMethod httpMethod, CancellationToken cancellationToken)
            where TRequest : class
            where TResponse : class
            where TParams : class
        {
            Uri url = GetUrl(relativeUrl, parameters);

            HttpContent content = GetContent(request);

            var requestMessage = new HttpRequestMessage(httpMethod, url) { Content = content };

            return SendAsync<TResponse>(requestMessage, cancellationToken);
        }

        [ItemNotNull]
        protected internal Task<TResponse> PostAsync<TRequest, TParams, TResponse>([NotNull] string relativeUrl, [CanBeNull] TParams parameters, [CanBeNull] TRequest request, CancellationToken cancellationToken)
            where TRequest : class
            where TResponse : class
            where TParams : class
        {
            return RequestAsync<TRequest, TParams, TResponse>(relativeUrl, parameters, request, HttpMethod.Post, cancellationToken);
        }

        [ItemNotNull]
        protected internal Task<TResponse> PutAsync<TRequest, TParams, TResponse>([NotNull] string relativeUrl, [CanBeNull] TParams parameters, [CanBeNull] TRequest request, CancellationToken cancellationToken)
            where TRequest : class
            where TResponse : class
            where TParams : class
        {
            return RequestAsync<TRequest, TParams, TResponse>(relativeUrl, parameters, request, HttpMethod.Put, cancellationToken);
        }

        private async Task EnsureSuccessStatusCode([NotNull] HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryGetErrorDescriptionAsync(response);

                response.Content?.Dispose();

                if (response.StatusCode == HttpStatusCode.Unauthorized ||
                    response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new NotAuthorizedException(response.ReasonPhrase, error);
                }

                throw new YandexApiException(response.StatusCode, response.ReasonPhrase, error);
            }
        }

        [ItemCanBeNull]
        private async Task<ErrorDescription> TryGetErrorDescriptionAsync([NotNull] HttpResponseMessage response)
        {
            try
            {
                return response.Content != null
                    ? await response.Content.ReadAsAsync<ErrorDescription>()
                    : null;
            }
            catch (SerializationException) //unexpected data in content
            {
                return null;
            }
        }
    }
}
