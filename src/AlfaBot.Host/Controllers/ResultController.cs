using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using AlfaBot.Host.Model;
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
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [BindProperties]
    [ExcludeFromCodeCoverage]
    public class ResultController : ControllerBase
    {
        private readonly IQuizResultRepository _resultRepository;

        /// <inheritdoc />
        public ResultController(IQuizResultRepository resultRepository)
        {
            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
        }

        /// <summary>
        /// Return Result of the Quiz
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<ResultDto>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<ResultDto>> Json()
        {
            var results = _resultRepository.All();
            var dto = Map(results);
            return Ok(dto);
        }

        /// <summary>
        /// Return Result of the Quiz
        /// </summary>
        /// <returns></returns>
        [HttpGet("{top}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<ResultDto>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<ResultDto>> Json([Required] [Range(1, 20)] int top)
        {
            var results = _resultRepository.All(top);
            var dto = Map(results);
            return Ok(dto);
        }

        private static IEnumerable<ResultDto> Map(IEnumerable<QuizResult> results) =>
            results.Select(r => new ResultDto
            {
                Name = r.User.Name,
                Phone = r.User.Phone,
                Points = r.Points,
                Seconds = r.Seconds,
                TelegramName = r.User.TelegramName
            });

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
    }
}