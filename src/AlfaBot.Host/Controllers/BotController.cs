using System;
using AlfaBot.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlfaBot.Host.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [BindProperties]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class BotController : ControllerBase
    {
        private readonly IAlfaBankBot _bot;

        public BotController(IAlfaBankBot bot)
        {
            _bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<string> Start()
        {
            _bot.Start();
            return Ok("Bot started");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<string> Stop()
        {
            _bot.Stop();
            return Ok("Bot stopped");
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<string> Ping(string message)
            => _bot.Ping() ? $"Pong {message}" : "Bot not receiving";
    }
}