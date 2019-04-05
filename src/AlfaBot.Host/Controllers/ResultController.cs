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
        [AllowAnonymous]
        [HttpGet("{top}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<ResultDto>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<ResultDto>> Json([Required] [Range(1, 20)] int top)
        {
            var results = _resultRepository.All(top);
            var dto = Map(results, true);
            return Ok(dto);
        }

        private static IEnumerable<ResultDto> Map(IEnumerable<QuizResult> results, bool mask = false) =>
            results.Select(r => new ResultDto
            {
                Name = r.User.Name,
                Phone = mask ? MaskMobile(r.User.Phone, 3, "****") : r.User.Phone,
                Points = r.Points,
                Seconds = r.Seconds,
                TelegramName = r.User.TelegramName
            });

        // Mask the mobile.
        // Usage: MaskMobile("13456789876", 3, "****") => "134****9876"
        private static string MaskMobile(string mobile, int startIndex, string mask)
        {
            if (string.IsNullOrEmpty(mobile))
                return string.Empty;

            var result = mobile;
            var maskLength = mask.Length;


            if (mobile.Length < startIndex) return result;

            result = mobile.Insert(startIndex, mask);
            result = result.Length >= startIndex + maskLength * 2
                ? result.Remove(startIndex + maskLength, maskLength)
                : result.Remove(startIndex + maskLength, result.Length - (startIndex + maskLength));

            return result;
        }

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