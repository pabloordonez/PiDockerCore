using Microsoft.Extensions.CommandLineUtils;
using Paradigm.Services.CLI;

namespace PiDockerCore
{
    [HelpOption(new[] { "-h", "--help", "-?" })]
    [VersionOption(new[] { "-v", "--version" })]
    public class ApplicationArguments
    {
        [ArgumentOption("-ho", "--host", "The mysql host name or ip. i.e. 192.168.1.100 or mysql-server. Default value: localhost", CommandOptionType.SingleValue)]
        public string Host { get; set; }

        [ArgumentOption("-d", "--database", "The mysql database name or ip.", CommandOptionType.SingleValue)]
        public string Database { get; set; }

        [ArgumentOption("-u", "--username", "The user name. i.e. root. Default value: root", CommandOptionType.SingleValue)]
        public string UserName { get; set; }

        [ArgumentOption("-p", "--password", "The user password.", CommandOptionType.SingleValue)]
        public string Pepe { get; set; }
    }
}