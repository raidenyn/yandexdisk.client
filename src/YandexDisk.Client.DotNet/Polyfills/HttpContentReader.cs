using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Formatting
{
    /// <summary>
    /// Extension for reading http conent as object
    /// </summary>
    public static class HttpContentReader
    {
        /// <summary>
        /// Read http content as object
        /// </summary>
        public static async Task<T> ReadAsAsync<T>(this HttpContent httpContent, MediaTypeFormatter[] formatters, CancellationToken cancellationToken)
        {
            var formatter = formatters.First();

            var stream = await httpContent.ReadAsStreamAsync();
            var obj = await formatter.ReadFromStreamAsync(typeof(T), stream, cancellationToken);

            return (T)obj;
        }
    }
}
