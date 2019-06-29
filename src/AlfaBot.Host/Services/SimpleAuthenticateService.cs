using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AlfaBot.Host.Services
{
    /// <inheritdoc />
    public class SimpleAuthenticateService : ISimpleAuthenticateService
    {
        private readonly ICredentialsRepository _userRepository;
        private readonly byte[] _key;

        /// <inheritdoc />
        public SimpleAuthenticateService(ICredentialsRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            
            var configuration1 = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var key = configuration1["AUTHKEY"];
            _key = Encoding.ASCII.GetBytes(key);
        }

        /// <inheritdoc />
        public string CheckUserCredentials(string userName, string password)
        {
            var user = _userRepository.GetSecureUser(userName);

            if (user == null) return null;

            var verificationResult =
                new PasswordHasher<Credential>().VerifyHashedPassword(user, user.HashedPassword, password);

            if (verificationResult != PasswordVerificationResult.Success) return null;

            // authentication successful. Generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = tokenHandler.WriteToken(token);
            return result;
        }
    }
}