using System;
using System.IO;
using System.Threading.Tasks;
using YandexDisk.Client.Http;

namespace YandexDisk.Client.CLI
{
    internal class Upload
    {
        public DiskHttpApi DiskClient { get; }

        public Upload(DiskHttpApi diskClient) {
            DiskClient = diskClient;
        }

        public async Task ExecuteAsync(UploadOptions options) {
            Console.WriteLine("Receiving upload link from yandex.disk...");

            var link = await DiskClient.Files.GetUploadLinkAsync(options.Target, true).ConfigureAwait(false);

            var sourceFileName = Path.Combine(Directory.GetCurrentDirectory(), options.Source);

            Console.WriteLine($"Uploading '{sourceFileName}' to yandex.disk...");

            using (var fileStream = File.OpenRead(sourceFileName))
            {
                await DiskClient.Files.UploadAsync(link, fileStream);
            }

            Console.WriteLine($"File '{sourceFileName}' was successfully to '{options.Target}' on yandex.disk.");
        }
    }
}
