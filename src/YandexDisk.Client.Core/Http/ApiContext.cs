using System;

namespace YandexDisk.Client.Http
{
    internal class ApiContext
    {
        public IHttpClient HttpClient { get; set; }
        
        public Uri BaseUrl { get; set; }

        public ILogSaver LogSaver { get; set; }
    }
}
