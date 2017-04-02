namespace YandexDisk.Client.Tests.Polyfils
{
    public static class UriPf
    {
        /// <summary>
        /// Polyfils for different escaping behavior in .NET 4.0 and 4.5
        /// </summary>
        public static string EscapePath(string path)
        {
            return path;
        }
    }
}
