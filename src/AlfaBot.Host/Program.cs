using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

#pragma warning disable 1591

// ReSharper disable ClassNeverInstantiated.Global

namespace AlfaBot.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .WriteTo.Console(
                            LogEventLevel.Warning,
                            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                        .WriteTo.RollingFile("./logs/alfabank-bot-{Hour}.log",
                            LogEventLevel.Debug,
                            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Properties}{NewLine}{Exception}")
                        .CreateLogger();
                })
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5000")
                .UseSerilog();
    }
}