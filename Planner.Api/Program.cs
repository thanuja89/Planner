using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Planner.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            try
            {
                var host = WebHost.CreateDefaultBuilder(args)
                    .CaptureStartupErrors(false)
                    .UseSetting("detailedErrors", "true")
                    .UseStartup<Startup>();

                return host;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
