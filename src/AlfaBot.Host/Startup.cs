using System;
using System.Net;
using AlfaBot.Core.Data;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Services;
using AlfaBot.Core.Services.Interfaces;
using AlfaBot.Host.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Telegram.Bot;

namespace AlfaBot.Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;

            var date = DateTime.Now;
            Console.WriteLine($"Hello, I'm a bot! Today is {date.DayOfWeek}, it's {date:HH:mm} now.");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure general params
            var token = _configuration["TELEGRAM_TOKEN"];
            var secretKey = _configuration["SECRETKEY"];
            var connection = _configuration.GetConnectionString("MONGO");
            var database = _configuration["DBNAME"] ?? "FindIT";

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(token, "Telegram token must be not null");

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentNullException(secretKey, "Secret key must be not null");

            if (string.IsNullOrWhiteSpace(connection))
                throw new ArgumentNullException(connection, "ConnectionString key must be not null");

            // configure proxy if it was present
            var proxyAddress = _configuration["PROXY_ADDRESS"];
            var proxyPort = _configuration["PROXY_PORT"];

            // configure db
            services.AddSingleton(_ => new MongoClient(connection).GetDatabase(database).Init());
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IQueueService, MongoQueueService>();
            services.AddSingleton<ICommandFactory, CommandFactory>();

            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token, GetProxy(proxyAddress, proxyPort)));
            services.AddSingleton<IAlfaBankBot, AlfaBankBot>();

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseAuthentication();

            app.UseMvc();

            app.ApplicationServices.GetService<IAlfaBankBot>().Start();
        }

        private static IWebProxy GetProxy(string proxyAddress, string proxyPort)
        {
            if (string.IsNullOrWhiteSpace(proxyAddress))
                return null;
            var isPortCorrect = int.TryParse(proxyPort, out var port);

            if (!(isPortCorrect && 0 < port && port <= 65536))
                return null;

            return new WebProxy(proxyAddress, port);
        }
    }
}