using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Services.Interfaces;
using AlfaBot.Host.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlfaBot.Host.Controllers
{
    /// <inheritdoc />
    [Authorize(Roles = "Administrators")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [BindProperties]
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
        [ProducesResponseType(typeof(StatusOutDto), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<StatusOutDto> Start()
        {
            try
            {
                _bot.Start();
            }
            catch
            {
                return StatusCode(500, new StatusOutDto {Status = "Bot was not started"});
            }

            return Ok(new StatusOutDto {Status = "Bot was started"});
        }

        /// <summary>
        /// Stop bot
        /// </summary>
        /// <returns>Status of bot stopping</returns>
        [HttpGet]
        [ProducesResponseType(typeof(StatusOutDto), StatusCodes.Status200OK)]
        public ActionResult<StatusOutDto> Stop()
        {
            _bot.Stop();
            return Ok(new StatusOutDto {Status = "Bot was stopped"});
        }

        /// <summary>
        /// Ping the bot status
        /// </summary>
        /// <returns>Status of the bot</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StatusOutDto), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<StatusOutDto> Ping()
        {
            var healthy = _bot.Ping();
            return healthy
                ? Ok(new StatusOutDto {Status = "Pong, Bot is receiving"})
                : StatusCode(500, new StatusOutDto {Status = "Bot is not receiving"});
        }
    }
}