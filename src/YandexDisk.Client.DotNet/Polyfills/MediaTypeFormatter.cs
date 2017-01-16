using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Formatting
{
    /// <summary>
    /// Polyfill for MediaTypeFormatter
    /// </summary>
    public abstract class MediaTypeFormatter
    {
        /// <summary>
        /// Polyfill for ReadFromStreamAsync
        /// </summary>
        public abstract Task<object> ReadFromStreamAsync(Type type, Stream readStream, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Polyfill for WriteToStreamAsync
        /// </summary>
        public abstract Task WriteToStreamAsync(Type type, object value, Stream writeStream, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Polyfill for JsonMediaTypeFormatter
    /// </summary>
    public class JsonMediaTypeFormatter: MediaTypeFormatter
    {
        /// <summary>
        /// Polyfill for SerializerSettings
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings();

        /// <summary>
        /// Polyfill for ReadFromStreamAsync
        /// </summary>
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, CancellationToken cancellationToken)
        {
            using (var textStream = new StreamReader(readStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true))
            {
                var obj = JsonSerializer.Create(SerializerSettings).Deserialize(textStream, type);
                return Task.FromResult(obj);
            }
        }

        /// <summary>
        /// Polyfill for WriteToStreamAsync
        /// </summary>
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, CancellationToken cancellationToken)
        {
            using (var textStream = new StreamWriter(writeStream, Encoding.UTF8, bufferSize: 4096, leaveOpen: true))
            {
                JsonSerializer.Create(SerializerSettings).Serialize(textStream, value, type);
                return Task.CompletedTask;
            }
        }
    }
}
