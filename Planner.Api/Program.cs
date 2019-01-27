using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

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
