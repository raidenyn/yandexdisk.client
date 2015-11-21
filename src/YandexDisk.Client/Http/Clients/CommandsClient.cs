using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients
{
    internal class CommandsClient : DiadocClientBase, ICommandsClient
    {
        internal CommandsClient(ApiContext apiContext)
            : base(apiContext)
        { }

        public Task<Link> CreateDictionaryAsync(string path, CancellationToken cancellationToken)
        {
            return PutAsync<object, object, Link>("resources", new { path }, null, cancellationToken);
        }
    }
}
