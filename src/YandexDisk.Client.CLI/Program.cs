using CommandLine;
using CommandLine.Text;
using System;
using System.Threading.Tasks;
using YandexDisk.Client.Http;

namespace YandexDisk.Client.CLI
{
    public class YandexDiskCliProgram
    {
        public static int Main(string[] args)
        {
            return new YandexDiskCliProgram().Run(args);
        }

        public int Run(string[] args)
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

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }

        private void Upload(UploadOptions options)
        {
            SyncAwait(new Upload(CreateDiskClient(options)).ExecuteAsync(options));
        }

        private void Download(DownloadOptions options)
        {
            SyncAwait(new Download(CreateDiskClient(options)).ExecuteAsync(options));
        }

        protected virtual DiskHttpApi CreateDiskClient(OptionsBase options) {
            if (String.IsNullOrWhiteSpace(options.AccessToken)) {
                throw new Exception("AccessToken is not defined.");
            }

            return new DiskHttpApi(options.AccessToken);
        }

        private void SyncAwait(Task task)
        {
            task.Wait();
        }
    }
}
