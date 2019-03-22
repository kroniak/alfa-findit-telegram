using System;
using System.Linq;
using System.Net;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using FindAlfaITBot.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FindAlfaITBot
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath);

            builder
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables("ASPNETCORE_");

            Configuration = builder.Build();

            var date = DateTime.Now;
            Console.WriteLine($"Hello, I'm a bot! Today is {date.DayOfWeek}, it's {date:HH:mm} now.");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var token = Configuration["TELEGRAM_TOKEN"];
            var secretKey = Configuration["SECRETKEY"];
            var connection = Configuration["MONGO"];

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Telegram token must be not null");

            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("Secret key must be not null");

            if (string.IsNullOrEmpty(connection))
                throw new ArgumentNullException("ConnectionString key must be not null");

            var proxyAddress = Configuration["PROXY_ADDRESS"];
            var proxyPort = Configuration["PROXY_PORT"];

            var proxy = GetProxy(proxyAddress, proxyPort);


            MongoDBHelperQuestion.ConfigureConnection(connection);

            InitDB();

            Console.WriteLine($"Connection string is {MongoDBHelperQuestion.GetConnectionName}");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<ITelegramBot>(_ => new FindITBot(token, secretKey, proxy).Start());
        }

        private WebProxy GetProxy(string proxyAddress, string proxyPort)
        {
            if (string.IsNullOrWhiteSpace(proxyAddress))
                return null;
            var isPortCorrect = int.TryParse(proxyPort, out var port);

            if (!(isPortCorrect && 0 < port && port <= 65536))
                return null;

            return new WebProxy(proxyAddress, port);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{message?}");
            });
        }

        private static void InitDB()
        {
            var people = MongoDBHelperQuestion.All().Result;

            if (people.Count() == 0)
            {
                var person = new Person
                {
                    EMail = "test@test.ru",
                    Name = "Test Student",
                    Profession = "Haskell",
                    University = "sgu"
                };

                MongoDBHelperQuestion.AddPerson(person);
                people = MongoDBHelperQuestion.All().Result;

                if (people.Count() == 0)
                    throw new Exception("Fail to start DB");
            }
        }
    }
}