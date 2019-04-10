using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable ClassNeverInstantiated.Global

namespace AlfaBot.Host.Middleware
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly string _secretKey;
        private readonly string _secretUserKey;

        /// <inheritdoc />
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (encoder == null) throw new ArgumentNullException(nameof(encoder));
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _secretKey = configuration["SECRETKEY"];
            _secretUserKey = configuration["SECRETKEY_USER"];
        }

        /// <inheritdoc />
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var secret = Request.Query["secretkey"];

            if (string.IsNullOrWhiteSpace(secret))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Key"));
            }

            Claim roleClaim;
            
            if (string.CompareOrdinal(_secretKey, secret) == 0)
            {
                roleClaim = new Claim(ClaimTypes.Role, "Administrators");
            }
            else if (string.CompareOrdinal(_secretUserKey, secret) == 0)
            {
                roleClaim = new Claim(ClaimTypes.Role, "Users");
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Key"));
            }

            var claims = new[]
            {
                roleClaim
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}