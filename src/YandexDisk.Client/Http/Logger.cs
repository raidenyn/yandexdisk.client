using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace YandexDisk.Client.Http
{
    internal interface ILogger: IDisposable
    {
        Task SetRequestAsync([NotNull] HttpRequestMessage request);

        Task SetResponseAsync([NotNull] HttpResponseMessage httpResponseMessage);

        void EndWithSuccess();

        void EndWithError([NotNull] Exception e);
    }

    internal class Logger : ILogger
    {
        private readonly ILogSaver _log;
        private readonly RequestLog _requestLog = new RequestLog();
        private readonly ResponseLog _responseLog = new ResponseLog();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        internal Logger(ILogSaver log)
        {
            _log = log;
        }

        public async Task SetRequestAsync(HttpRequestMessage request)
        {
            _requestLog.Headers = request.ToString();
            if (request.Content != null)
            {
                _requestLog.Body = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }

            _requestLog.StartedAt = DateTime.Now;
            _stopwatch.Start();
        }

        public async Task SetResponseAsync(HttpResponseMessage httpResponseMessage)
        {
            _stopwatch.Stop();

            _responseLog.Headers = httpResponseMessage.ToString();
            _responseLog.StatusCode = httpResponseMessage.StatusCode;

            if (httpResponseMessage.Content != null)
            {
                _responseLog.Body = await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
        }

        public void EndWithSuccess()
        {
            _responseLog.Duration = _stopwatch.ElapsedMilliseconds;

            SaveLog();
        }

        public void EndWithError(Exception e)
        {
            _responseLog.Exception = e.Message;
            _responseLog.Duration = _stopwatch.ElapsedMilliseconds;

            SaveLog();
        }

        private void SaveLog()
        {
            _log?.SaveLog(_requestLog, _responseLog);

            _isDisposed = true;
        }

        private bool _isDisposed;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                EndWithError(new Exception("Log object is never ended. You should end log befor disposing."));
            }
        }
    }

    internal class DummyLogger : ILogger
    {
        public void Dispose()
        { }

        public Task SetRequestAsync(HttpRequestMessage request)
        {
            return TaskPf.CompletedTask;
        }

        public Task SetResponseAsync(HttpResponseMessage httpResponseMessage)
        {
            return TaskPf.CompletedTask;
        }

        public void EndWithSuccess()
        { }

        public void EndWithError(Exception e)
        { }
    }

    internal static class LoggerFactory
    {
        public static ILogger GetLogger([CanBeNull] ILogSaver saver)
        {
            return saver != null ? (ILogger)new Logger(saver) : new DummyLogger();
        }
    }
}
