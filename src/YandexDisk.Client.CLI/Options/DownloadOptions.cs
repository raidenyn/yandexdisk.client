using CommandLine;

namespace YandexDisk.Client.CLI
{
    [Verb("download")]
    class DownloadOptions : OptionsBase
    {
        [Value(1, HelpText = "File or directory on Yandex.Disk")]
        public string Source { get; set; }

        [Value(2, HelpText = "Local folder")]
        public string Target { get; set; }

        [Option('u', "unzip", HelpText = "Unzip folder during download")]
        public bool Unzip { get; set; }
    }
}
