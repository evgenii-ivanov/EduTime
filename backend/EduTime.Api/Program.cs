using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduTime.Api
{
    public class Program
    {
	    public static void Main(string[] args)
	    {
		    CreateHostBuilder(args).Build().Run();
	    }

	    public static IHostBuilder CreateHostBuilder(string[] args) =>
		    Host.CreateDefaultBuilder(args)
			    .ConfigureAppConfiguration(config =>
			    {
				    config.AddJsonFile("appsettings.local.json", optional: true);
				    config.AddEnvironmentVariables("DS_");
			    })
			    .ConfigureWebHostDefaults(webBuilder =>
			    {
				    webBuilder.UseStartup<Startup>();
			    });
    }
}
