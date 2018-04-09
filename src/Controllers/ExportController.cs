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
        public IActionResult Index(string secretKey)
        {
            if (String.CompareOrdinal(_bot.SecretKey, secretKey) != 0)
                return new ForbidResult();

            return Json(MongoDBHelper.All().Result);
        }

        [HttpGet]
        public IActionResult Csv(string secretKey)
        {
            if (String.CompareOrdinal(_bot.SecretKey, secretKey) != 0)
                return new ForbidResult();

            var sb = new StringBuilder();
            sb.AppendLine("EMail;Name;Profession;University;");

            var students = MongoDBHelper.All().Result;
            if (students.Count() > 0)
            {
                foreach (var stud in students)
                {
                    sb.AppendLine($"{stud.EMail};{stud.Name};{stud.Profession};{stud.University};");
                }
            }

            return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "data.csv");
        }
    }
}