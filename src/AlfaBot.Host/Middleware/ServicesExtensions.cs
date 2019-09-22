using System.Diagnostics.CodeAnalysis;
using System.Text;
using AlfaBot.Core.Data;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Factories.Commands;
using AlfaBot.Core.Services;
using AlfaBot.Core.Services.Interfaces;
using AlfaBot.Host.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

// ReSharper disable UnusedMethodReturnValue.Global
#pragma warning disable 1591

namespace AlfaBot.Host.Middleware
{
    [ExcludeFromCodeCoverage]
    public static class ServicesExtensions
    {
        public static IServiceCollection AddRepositoryAndServices(this IServiceCollection services)
        {
            services
                .AddSingleton<ICredentialsRepository, CredentialsRepository>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IQueueService, MongoQueueService>()
                .AddSingleton<ILogRepository, LogRepository>()
                .AddSingleton<IGeneralCommandsFactory, GeneralCommandsFactory>()
                .AddSingleton<ISimpleAuthenticateService, SimpleAuthenticateService>();

            return services;
        }

        /// <summary>
        /// Add custom Auth middleware
        /// </summary>
        /// <param name="services">IService Collections</param>
        /// <param name="key">Global key configuration</param>
        public static void AddCustomAuthentication(this IServiceCollection services, string key)
        {
            var keyEncoded = Encoding.ASCII.GetBytes(key);
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("Administrators",
                        policy => { policy.RequireRole("Administrators"); });
                    options.AddPolicy("Users",
                        policy => { policy.RequireRole("Users"); });
                })
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(keyEncoded),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }
    }
}