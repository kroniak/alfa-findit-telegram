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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace AlfaBot.Host.Controllers
{
    /// <inheritdoc />
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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
        /// Get Result information for user with json result
        /// </summary>
        /// <param name="chatId">ChatId long number</param>
        /// <returns>Result information for user with json result</returns>
        [HttpGet("{chatId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ResultOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<ResultOutDto> Get([Required] [Range(1, long.MaxValue)] long chatId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            QuizResult quizResult;

            try
            {
                quizResult = _resultRepository.Get(chatId);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (quizResult != null)
            {
                return Ok(MapOne(quizResult));
            }

            return NotFound();
        }

        /// <summary>
        /// Return Result of the Quiz with json or csv
        /// </summary>
        /// <returns>Result of the Quiz with json or csv</returns>
        [HttpGet]
        [Produces("application/json", "text/csv")]
        [ProducesResponseType(typeof(IEnumerable<ResultOutDto>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<ResultOutDto>> Get()
        {
            var results = _resultRepository.All();
            var dto = Map(results);
            return Ok(dto);
        }

        /// <summary>
        /// Return Result of the Quiz
        /// </summary>
        /// <returns>Result of the Quiz</returns>
        [AllowAnonymous]
        [HttpGet("top/{top:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<ResultOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<ResultOutDto>> Get([Required] [Range(1, 30)] int top)
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

        private static IEnumerable<ResultOutDto> Map(IEnumerable<QuizResult> results, bool mask = false) =>
            results.Select(r => MapOne(r, mask)).ToArray();

        private static ResultOutDto MapOne(QuizResult result, bool mask = false) =>
            new ResultOutDto
            {
                Name = result.User.Name,
                Phone = mask ? MaskMobile(result.User.Phone, 3, "****") : result.User.Phone,
                Points = result.Points,
                Seconds = CalcSeconds(result.Ended, result.Started),
                TelegramName = result.User.TelegramName
            };

        private static int CalcSeconds(DateTime ended, DateTime started)
        {
            if (ended >= started) return (int) Math.Round((ended - started).TotalSeconds);

            var seconds = Math.Round((DateTime.Now - started).TotalSeconds);
            return (int) (seconds >= int.MaxValue ? int.MaxValue : seconds);
        }

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