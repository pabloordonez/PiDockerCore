using System;
using Paradigm.Services.CLI;

namespace PiDockerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = ConsoleHost.Create();

            host.ParseArguments<ApplicationArguments>(args)
                .SetVersion("1.0", "1.0.0");

            host.UseStartup<Startup>()
                .Build();

            if (host.HostingEnvironment.IsDevelopment())
            {
                Console.ReadKey();
            }
        }
    }
}
