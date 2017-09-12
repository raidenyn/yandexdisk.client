using CommandLine;

namespace YandexDisk.Client.CLI
{
    [Verb("upload")]
    class UploadOptions : OptionsBase
    {
        [Value(1, HelpText = "Local file or directory")]
        public string Source { get; set; }

        [Value(2, HelpText = "Target directory on Yabndex.Disk")]
        public string Target { get; set; }
    }
}
