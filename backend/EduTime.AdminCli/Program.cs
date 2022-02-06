using CommandLine;
using EduTime.AdminCli.CliOptions;
using EduTime.AdminCli.Facades;
using Microsoft.Extensions.Configuration;

namespace EduTime.AdminCli
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.{Environment}.json", true)
                .AddJsonFile("appsettings.local.json", true)
                .AddEnvironmentVariables()
                .Build();

            Parser.Default.ParseArguments<MigrationOptions>(args)
                .MapResult(
                    options => MigrationFacade.Run(options, configuration),
                    errors => 1
                );
        }
    }
}
