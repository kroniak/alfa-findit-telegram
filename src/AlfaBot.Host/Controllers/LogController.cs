using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
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
    [Produces("application/json")]
    [BindProperties]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ExcludeFromCodeCoverage]
    public class LogController : ControllerBase
    {
        private readonly ILogRepository _logRepository;
        private readonly ILogger<LogController> _logger;

        /// <inheritdoc />
        public LogController(
            ILogRepository logRepository,
            ILogger<LogController> logger)
        {
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get log records from global log
        /// </summary>
        /// <param name="messageId">MessageId int value</param>
        /// <returns>Returns LogRecords list</returns>
        /// <response code="200">Returns successfully log record</response>
        /// <response code="400">Return bad requests</response>
        [HttpGet("{messageId}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(IEnumerable<LogRecord>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<LogRecord>> Message([Required] int messageId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            var result = _logRepository.GetRecords(messageId);

            return Ok(result);
        }

        /// <summary>
        /// Get log records from global log
        /// </summary>
        /// <param name="chatId">ChatId long value</param>
        /// <param name="top">Set top limit with DESC sorting</param>
        /// <returns>Returns LogRecords list</returns>
        /// <response code="200">Returns successfully log record</response>
        /// <response code="400">Return bad requests</response>
        [HttpGet("{chatId}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(IEnumerable<LogRecord>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<LogRecord>> Chat(
            [Required] [Range(1, long.MaxValue)] long chatId, 
            [Range(1, int.MaxValue)] int? top)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            var result = _logRepository.GetRecords(chatId, top);

            return Ok(result);
        }

        /// <summary>
        /// Get All log records from global log
        /// </summary>
        /// <param name="top">Set top limit with DESC sorting</param>
        /// <returns>Returns LogRecords list</returns>
        /// <response code="200">Returns successfully log record</response>
        [HttpGet]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(IEnumerable<LogRecord>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<LogRecord>> All([Range(1, int.MaxValue)] int? top)
        {
            var result = _logRepository.All(top);

            return Ok(result);
        }
    }
}