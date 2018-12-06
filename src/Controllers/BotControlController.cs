using System;
using FindAlfaITBot.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FindAlfaITBot.Controllers
{
    public class BotControlController : Controller
    {
        private readonly ITelegramBot _bot;

        public BotControlController(ITelegramBot bot)
        {
            _bot = bot;
        }

        [HttpGet]
        public IActionResult Start(string secretKey)
        {
            if (String.CompareOrdinal(_bot.SecretKey, secretKey) != 0)
                return StatusCode(403);

            _bot.Start();
            return Json("Bot startted");
        }

        [HttpGet]
        public IActionResult Stop(string secretKey)
        {
            if (String.CompareOrdinal(_bot.SecretKey, secretKey) != 0)
                return StatusCode(403);

            _bot.Stop();
            return Json("Bot stopped");
        }

        [HttpGet]
        public string Ping(string message)
            => _bot.Ping() ? $"Pong {message}" : "Bot not receiving";
    }
}