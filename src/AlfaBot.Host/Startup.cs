using System;
using System.IO;
using System.Net;
using System.Reflection;
using AlfaBot.Core.Data;
using AlfaBot.Core.Services;
using AlfaBot.Core.Services.Interfaces;
using AlfaBot.Host.HealthCheckers;
using AlfaBot.Host.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Swagger;
using Telegram.Bot;
using WebApiContrib.Core.Formatter.Csv;

// ReSharper disable UnusedMember.Global

#pragma warning disable 1591

namespace AlfaBot.Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure general params
            var token = _configuration["TELEGRAM_TOKEN"];
            var authKey = _configuration["AUTHKEY"];
            var adminPass = _configuration["ADMINPASS"];
            var userPass = _configuration["USERPASS"];
            var connection = _configuration["MONGO"];
            var database = _configuration["DBNAME"] ?? "FindIT";

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(token, "Telegram token must be not null");

            if (string.IsNullOrWhiteSpace(authKey)
                || string.IsNullOrWhiteSpace(adminPass)
                || string.IsNullOrWhiteSpace(userPass))
                throw new ArgumentNullException(authKey, "Secret key must be not null");

            if (string.IsNullOrWhiteSpace(connection))
                throw new ArgumentNullException(connection, "ConnectionString key must be not null");

            // configure proxy if it was present
            var proxyAddress = _configuration["PROXY_ADDRESS"];
            var proxyPort = _configuration["PROXY_PORT"];

            // configure db
            services.AddSingleton(_ => new MongoClient(connection).GetDatabase(database).Init(adminPass, userPass));

            // add repositories
            services.AddMemoryCache();
            services.AddRepositoryAndServices();

            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token, GetProxy(proxyAddress, proxyPort)));
            services.AddSingleton<IAlfaBankBot, AlfaBankBot>();

            services.AddCustomAuthentication(authKey);

            // Add Background service
            services.AddHostedService<SendingHostedService>();

            // Add MVC and other services
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(
                                "http://bot.kroniak.net",
                                "https://bot.kroniak.net",
                                "http://localhost:3000")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddCsvSerializerFormatters();

            services.AddSwaggerGen(
                options =>
                {
                    // add a custom operation filter which sets default values
                    options.SwaggerDoc(
                        "v2",
                        new Info
                        {
                            Title = "Alfabank Bot API",
                            Version = "v2",
                            Description = "A ASP.NET Core Web API for Alfabank Bot Host Application",
                            Contact = new Contact
                            {
                                Name = "Nikolay Molchanov",
                                Email = "me@kroniak.net",
                                Url = "https://github.com/kroniak"
                            },
                            TermsOfService = "Shareware",
                            License = new License {Name = "MIT", Url = "https://opensource.org/licenses/MIT"}
                        });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    // integrate xml comments
                    options.IncludeXmlComments(xmlPath);
                });

            services.AddCustomHealthChecks(_configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IAlfaBankBot bot)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=600");
                }
            });

            app.UseCustomHealthCheckEndpoints();

            app.UseAuthentication();

            app.UseSwagger();

            app.UseSwaggerUI(
                options => { options.SwaggerEndpoint("/swagger/v2/swagger.json", "Bot API v2"); });

            app.UseCors();
            
            app.UseMvc();

            bot.Start();
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