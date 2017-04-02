using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpContentExtensionsPolyfils
    {
        public static Task<T> ReadAsAsync<T>(this HttpContent content, IEnumerable<MediaTypeFormatter> formatters, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return content.ReadAsAsync<T>(formatters);
        }
    }
}
