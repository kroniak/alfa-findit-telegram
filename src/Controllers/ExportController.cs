using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using FindAlfaITBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindAlfaITBot.Controllers
{
    public class ExportController : Controller
    {
        private readonly ITelegramBot _bot;

        public ExportController(ITelegramBot bot)
        {
            _bot = bot;
        }

        [HttpGet]
        public IActionResult Json(string secretKey)
        {
            if (String.CompareOrdinal(_bot.SecretKey, secretKey) != 0)
                return StatusCode(403);

            return Ok(MongoDBHelper.All().Result);
        }

        [HttpGet]
        public IActionResult Csv(string secretKey)
        {
            if (String.CompareOrdinal(_bot.SecretKey, secretKey) != 0)
                return StatusCode(403);

            var sb = new StringBuilder();
            sb.AppendLine("EMail;Name;Profession;University;Course;Phone;Telegram;");

            var people = MongoDBHelper.All().Result;
            if (people.Count() > 0)
            {
                foreach (var p in people)
                {
                    sb.AppendLine($"{p.EMail};{p.Name};{p.Profession};{p.University};{p.Course};{p.Phone};{p.TelegramName};");
                }
            }

            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "data.csv");
        }

        [HttpGet]
        public IActionResult JsonResults(string secretKey)
        {
            if (String.CompareOrdinal(_bot.SecretKey, secretKey) != 0)
                return StatusCode(403);

            return Json(MongoDBHelper.AllResults().Result);
        }

        [HttpGet]
        public IActionResult CsvResult(string secretKey)
        {
            if (String.CompareOrdinal(_bot.SecretKey, secretKey) != 0)
                return StatusCode(403);

            var sb = new StringBuilder();

            sb.AppendLine("Name;Phone;Telegram;Points");

            var results = MongoDBHelper.AllResults().Result;

            foreach (var res in results)
            {
                sb.AppendLine($"{res.Person.Name};{res.Person.Phone};{res.Person.TelegramName};{res.Points};");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "results.csv");
        }
    }
}