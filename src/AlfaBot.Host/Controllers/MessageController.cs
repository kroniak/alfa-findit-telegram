using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Services.Interfaces;
using AlfaBot.Host.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace AlfaBot.Host.Controllers
{
    /// <inheritdoc />
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [BindProperties]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]    
    [ExcludeFromCodeCoverage]
    public class MessageController : ControllerBase
    {
        private readonly IAlfaBankBot _bot;
        private readonly ILogger<MessageController> _logger;

        /// <inheritdoc />
        public MessageController(IAlfaBankBot bot, ILogger<MessageController> logger)
        {
            _bot = bot ?? throw new ArgumentNullException(nameof(bot));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Add message to handler without real client
        /// </summary>
        /// <param name="message">Message which received from client
        /// {
        ///    "chatId": "1234567",
        ///    "text": "Hello",
        /// }</param>
        /// <returns>True or false of the handled status</returns>
        /// <response code="200">Returns successfully status</response>
        /// <response code="400">Return bad handling status</response>
        [HttpPost]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddText([FromBody] TextMessageDto message)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            var messageEvent = new Message
            {
                Text = message.Text,
                Chat = new Chat
                {
                    Id = message.ChatId
                },
                Date = DateTime.Now
            };

            var result = _bot.MessageHandler(messageEvent);

            return result ? (IActionResult) Ok() : BadRequest();
        }
    }
}