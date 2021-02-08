using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ConfigurationServerLib;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {                    
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddRedisConfigServer();
                });
    }    
}
