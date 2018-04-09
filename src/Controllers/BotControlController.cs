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
        public string Start()
        {
            _bot.Start();
            return "startted";
        }

        [HttpGet]
        public string StopBot()
        {
            _bot.Stop();
            return "Bot stopped";
        }

        [HttpGet]
        public string Ping(string message)
        => _bot.Ping() ? $"Pong {message}" : "Bot not receiving";
    }
}