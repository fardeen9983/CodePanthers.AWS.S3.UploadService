using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodePanthers.AWS.S3.UploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Run();
        }

        public static IWebHost CreateHostBuilder(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builder, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false,reloadOnChange : true);
                })
                .Build();

    }
}
