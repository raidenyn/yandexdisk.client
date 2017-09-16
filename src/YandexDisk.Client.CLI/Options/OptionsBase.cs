using CommandLine;

namespace YandexDisk.Client.CLI
{
    public class OptionsBase
    {
        [Option('t', "access-token", Required = true, HelpText = "Yandex application access-token")]
        public string AccessToken { get; set; }

        [Value(0, HelpText = "Console command")]
        public string Verb { get; set; }
    }
}
