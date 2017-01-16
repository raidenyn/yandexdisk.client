using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http.Formatting
{
    /// <summary>
    /// Polyfill for ObjectContent
    /// </summary>
    public class ObjectContent<T> : HttpContent
    {
        private readonly MediaTypeFormatter _formatter;
        private readonly T _obj;
        private long? _length;

        /// <summary>
        /// Polyfill for ObjectContent
        /// </summary>
        public ObjectContent(T obj, MediaTypeFormatter formatter)
        {
            _obj = obj;
            _formatter = formatter;
        }

        /// <summary>
        /// Polyfill for SerializeToStreamAsync
        /// </summary>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var position = stream.Position;
            await _formatter.WriteToStreamAsync(typeof(T), _obj, stream).ConfigureAwait(false);
            _length = stream.Position - position;
        }

        /// <summary>
        /// Polyfill for TryComputeLength
        /// </summary>
        protected override bool TryComputeLength(out long length)
        {
            if (_length.HasValue)
            {
                length = _length.Value;
                return true;
            }
            length = 0;
            return false;
        }
    }
}
