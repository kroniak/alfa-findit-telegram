//using System;
//using System.Text;
//using FindAlfaITBot.Models;
//using FindAlfaITBot.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Question = FindAlfaITBot.Infrastructure.Question;
//
//namespace FindAlfaITBot.Controllers
//{
//    [Authorize]
//    [ApiController]
//    [Route("api/[controller]/[action]")]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    [BindProperties]
//    public class ExportController : ControllerBase
//    {
//        private readonly IAlfaBankBot _bot;
//
//        public ExportController(IAlfaBankBot bot)
//        {
//            _bot = bot ?? throw new ArgumentNullException(nameof(bot));
//        }
//
//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
//        [ProducesDefaultResponseType]
//        public IActionResult Json()
//        {
//            var result = Question.All().Result;
//
//            return Ok(result);
//        }
//
//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
//        [ProducesDefaultResponseType]
//        public IActionResult Csv()
//        {
//            var sb = new StringBuilder();
//            sb.AppendLine("EMail;Name;Profession;University;Course;Phone;Telegram;");
//
//            var people = Question.All().Result;
//
//
//            foreach (var p in people)
//            {
//                sb.AppendLine(
//                    $"{p.EMail};{p.Name};{p.Profession};{p.University};{p.Course};{p.Phone};{p.TelegramName};");
//            }
//
//            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "data.csv");
//        }
//
//        [HttpGet]
//        public IActionResult JsonResults()
//        {
//            return Json(Question.AllResults().Result);
//        }
//
//        [HttpGet]
//        public IActionResult CsvResult()
//        {
//            var sb = new StringBuilder();
//
//            sb.AppendLine("Name;Phone;Telegram;Points");
//
//            var results = Question.AllResults().Result;
//
//            foreach (var res in results)
//            {
//                sb.AppendLine($"{res.User.Name};{res.User.Phone};{res.User.TelegramName};{res.Points};");
//            }
//
//            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "results.csv");
//        }
//    }
//}

