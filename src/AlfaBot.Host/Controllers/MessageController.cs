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
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class MessageController : ControllerBase
    {
        private readonly IAlfaBankBot _bot;

        public MessageController(IAlfaBankBot bot)
        {
            _bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }

        [HttpPost]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public ActionResult<string> AddText(long chatId, string message)
        {
            
            return Ok("Bot started");
        }
    }
}