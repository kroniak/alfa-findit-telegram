using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Host.Models;
using AlfaBot.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// ReSharper disable PossibleMultipleEnumeration
namespace AlfaBot.Host.Controllers
{
    /// <inheritdoc />
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [BindProperties]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ISimpleAuthenticateService _authenticateService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authenticateService">Default authenticated service</param>
        /// <param name="logger">Current Logger</param>
        [ExcludeFromCodeCoverage]
        public AuthController(
            ISimpleAuthenticateService authenticateService,
            ILogger<AuthController> logger)
        {
            _authenticateService = authenticateService ?? throw new ArgumentNullException(nameof(authenticateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST /auth/login
        /// <summary>
        /// Login user
        /// </summary>
        /// <returns>A `string` type with token</returns>
        /// <response code="200">Login user successfully</response>
        [Route("/auth/login")]
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public ActionResult<string> Login([FromBody] CredentialDto userDto)
        {
            // try to validate user model
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This login model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            // try to verify user data
            var token = _authenticateService.CheckUserCredentials(userDto.Username, userDto.Password);

            // ReSharper disable once InvertIf
            if (token == null)
            {
                _logger.LogWarning("Authenticating is failed.");
                return Unauthorized();
            }

            // Return
            return Ok(new {token});
        }

        // POST /auth/login
        /// <summary>
        /// Verify user JWT
        /// </summary>
        /// <response code="200">Login user successfully</response>
        [Route("/auth/verify")]
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        public ActionResult Verify()
        {
            // Return
            return Ok(new {userName = User.Identity.Name});
        }
    }
}