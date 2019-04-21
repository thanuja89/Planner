using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
            var host = WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .UseSetting("detailedErrors", "true")
                .UseStartup<Startup>();

            return host;
        }
    }
}
