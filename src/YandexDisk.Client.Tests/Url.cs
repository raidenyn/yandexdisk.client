using System.Linq;

namespace YandexDisk.Client.Tests
{
    public static class Url
    {
        /// <summary>
        /// Polyfils for different escaping behavior in .NET 4.0 and 4.5
        /// </summary>
        public static string EscapePath(string url)
        {
            if (url == null)
            {
                return url;
            }

            var parts = url.Split('?');

            string path = parts[0];
            string query = string.Join("?", parts.Skip(1));

            return path + "?" + query.Replace("/", "%2F").Replace(",", "%2C");
        }
    }
}
