using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YandexDisk.Client.Http
{
    internal class RealHttpClientWrapper: IHttpClient
    {
        public RealHttpClientWrapper(HttpMessageInvoker httpMessageInvoker)
        {
            HttpMessageInvoker = httpMessageInvoker;
        }

        private HttpMessageInvoker HttpMessageInvoker { get; }


        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return HttpMessageInvoker.SendAsync(request, cancellationToken);
        }

        public void Dispose()
        {
            HttpMessageInvoker.Dispose();
        }
    }
}
