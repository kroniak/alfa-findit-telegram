using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using AlfaBot.Host.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AlfaBot.Host.Controllers
{
    /// <inheritdoc />
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [BindProperties]
    [ExcludeFromCodeCoverage]
    public class ResultController : ControllerBase
    {
        private readonly IQuizResultRepository _resultRepository;
        private readonly ILogger<ResultController> _logger;

        /// <inheritdoc />
        public ResultController(
            IQuizResultRepository resultRepository,
            ILogger<ResultController> logger)
        {
            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        public ActionResult<IEnumerable<ResultDto>> Json([Required] [Range(1, 30)] int top)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            var results = _resultRepository.All(top);
            var dto = Map(results, true);
            return Ok(dto);
        }

        /// <summary>
        /// Return Result of the Quiz information with csv result 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("text/csv")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public IActionResult Csv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name;Phone;Telegram;Points");

            var results = _resultRepository.All();
            var dto = Map(results);

            foreach (var d in dto)
            {
                sb.AppendLine($"{d.Name};{d.Phone};{d.TelegramName};{d.Points};");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "results.csv");
        }

        private static IEnumerable<ResultDto> Map(IEnumerable<QuizResult> results, bool mask = false) =>
            results.Select(r => new ResultDto
            {
                Name = r.User.Name,
                Phone = mask ? MaskMobile(r.User.Phone, 3, "****") : r.User.Phone,
                Points = r.Points,
                Seconds = (int) Math.Round((r.Ended - r.Started).TotalSeconds),
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
    }
}