using CommandLine;
using CommandLine.Text;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using YandexDisk.Client.Http;

namespace YandexDisk.Client.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parseResult = Parser.Default.ParseArguments<UploadOptions, DownloadOptions>(args)
                    .WithParsed<DownloadOptions>(Download)
                    .WithParsed<UploadOptions>(Upload);

                if (parseResult.Tag == ParserResultType.NotParsed)
                {
                    Console.WriteLine(HelpText.AutoBuild(parseResult));
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Upload(UploadOptions options)
        {
            var diskClient = new DiskHttpApi(options.AccessToken);

            Console.WriteLine("Receiving upload link from yandex.disk...");

            var link = SyncAwait(diskClient.Files.GetUploadLinkAsync(options.Target, true));

            var sourceFileName = Path.Combine(Directory.GetCurrentDirectory(), options.Source);

            Console.WriteLine($"Uploading '{sourceFileName}' to yandex.disk...");

            using (var fileStream = File.OpenRead(sourceFileName))
            {
                SyncAwait(diskClient.Files.UploadAsync(link, fileStream));
            }

            Console.WriteLine($"File '{sourceFileName}' was successfully to '{options.Target}' on yandex.disk.");
        }

        private static void Download(DownloadOptions options)
        {
            var diskClient = new DiskHttpApi(options.AccessToken);

            Console.WriteLine($"Receiving resource info from yandex.disk...");

            var resource = SyncAwait(diskClient.MetaInfo.GetInfoAsync(new Protocol.ResourceRequest() { Path = options.Source }));
            if (resource == null)
                throw new Exception($"Resource {options.Source} does not exist on disk");

            Console.WriteLine($"Receiving download link from yandex.disk...");

            var link = SyncAwait(diskClient.Files.GetDownloadLinkAsync(options.Source));

            Console.WriteLine($"Dowloading '{options.Source}' from yandex.disk...");

            var targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), options.Target);

            using (var downloadStream = SyncAwait(diskClient.Files.DownloadAsync(link)))
            {               
                switch (resource.Type)
                {
                    case Protocol.ResourceType.Dir:
                        if (options.Unzip)
                            SaveStreamToDirectory(downloadStream, targetDirectory);
                        else
                            SaveStreamToFile(downloadStream, Path.Combine(targetDirectory, resource.Name + ".zip"));
                        Console.WriteLine($"Directory '{options.Source}' was successfully saved in '{targetDirectory}'.");
                        break;
                    case Protocol.ResourceType.File:
                        SaveStreamToFile(downloadStream, Path.Combine(targetDirectory, resource.Name));
                        Console.WriteLine($"File '{options.Source}' was successfully saved in '{targetDirectory}'.");
                        break;
                }
            }
        }

        private static void SaveStreamToDirectory(Stream downloadStream, string targetDirectory)
        {
            ZipArchive archive = new ZipArchive(downloadStream, ZipArchiveMode.Read);
            archive.ExtractToDirectory(targetDirectory);
        }

        private static void SaveStreamToFile(Stream downloadStream, string fileName)
        {
            using (var fileStream = File.OpenWrite(fileName))
            {
                downloadStream.CopyTo(fileStream);
            }
        }

        private static TResult SyncAwait<TResult>(Task<TResult> task)
        {
            task.Wait();
            return task.Result;
        }

        private static void SyncAwait(Task task)
        {
            task.Wait();
        }
    }
}
