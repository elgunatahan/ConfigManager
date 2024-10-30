using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ConfigurationApi
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

                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        if (webBuilder.GetSetting("environment") == Environments.Development)
                        {
                            serverOptions.ListenAnyIP(8082);
                        }
                        else
                        {
                            serverOptions.ListenAnyIP(8083, listenOptions =>
                            {
                                listenOptions.UseHttps();
                            });
                        }
                    });
                });
    }
}
