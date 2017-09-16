using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using YandexDisk.Client.Http;

namespace YandexDisk.Client.CLI
{
    internal class Download
    {
        public DiskHttpApi DiskClient { get; }

        public Download(DiskHttpApi diskClient) {
            DiskClient = diskClient;
        }

        public async Task ExecuteAsync(DownloadOptions options) {
            Console.WriteLine($"Receiving resource info from yandex.disk...");

            var resource = await DiskClient.MetaInfo.GetInfoAsync(new Protocol.ResourceRequest() { Path = options.Source }).ConfigureAwait(false);
            if (resource == null)
            {
                throw new Exception($"Resource {options.Source} does not exist on disk");
            }

            Console.WriteLine($"Receiving download link from yandex.disk...");

            var link = await DiskClient.Files.GetDownloadLinkAsync(options.Source).ConfigureAwait(false);

            Console.WriteLine($"Dowloading '{options.Source}' from yandex.disk...");

            var targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), options.Target);

            using (var downloadStream = await DiskClient.Files.DownloadAsync(link).ConfigureAwait(false))
            {
                switch (resource.Type)
                {
                    case Protocol.ResourceType.Dir:
                        if (options.Unzip)
                        {
                            SaveStreamToDirectory(downloadStream, targetDirectory);
                        }
                        else
                        {
                            SaveStreamToFile(downloadStream, Path.Combine(targetDirectory, resource.Name + ".zip"));
                        }
                        Console.WriteLine($"Directory '{options.Source}' was successfully saved in '{targetDirectory}'.");
                        break;
                    case Protocol.ResourceType.File:
                        SaveStreamToFile(downloadStream, Path.Combine(targetDirectory, resource.Name));
                        Console.WriteLine($"File '{options.Source}' was successfully saved in '{targetDirectory}'.");
                        break;
                }
            }
        }

        private void SaveStreamToDirectory(Stream downloadStream, string targetDirectory)
        {
            var archive = new ZipArchive(downloadStream, ZipArchiveMode.Read);
            archive.ExtractToDirectory(targetDirectory);
        }

        private void SaveStreamToFile(Stream downloadStream, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                downloadStream.CopyTo(fileStream);
            }
        }
    }
}
