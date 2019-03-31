using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlfaBot.Host.Controllers
{
    /// <inheritdoc />
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [BindProperties]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ExcludeFromCodeCoverage]
    public class BotController : ControllerBase
    {
        private readonly IAlfaBankBot _bot;

        /// <inheritdoc />
        public BotController(IAlfaBankBot bot)
        {
            _bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }

        /// <summary>
        /// Start bot
        /// </summary>
        /// <returns>Status of bot starting</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public IActionResult Start()
        {
            try
            {
                _bot.Start();
            }
            catch
            {
                StatusCode(500, new {status = "Bot was not started"});
            }

            return Ok(new {status = "Bot was started"});
        }

        /// <summary>
        /// Stop bot
        /// </summary>
        /// <returns>Status of bot stopping</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public IActionResult Stop()
        {
            _bot.Stop();
            return Ok(new {status = "Bot was stopped"});
        }

        /// <summary>
        /// Ping the bot status
        /// </summary>
        /// <returns>Status of the bot</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public IActionResult Ping()
        {
            var healthy = _bot.Ping();
            return healthy
                ? Ok(new {status = "Pong, Bot is receiving"})
                : StatusCode(500, new {status = "Bot is not receiving"});
        }
    }
}