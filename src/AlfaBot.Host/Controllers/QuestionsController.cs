using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using AlfaBot.Host.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace AlfaBot.Host.Controllers
{
    /// <inheritdoc />
    [Authorize(Roles = "Administrators")]
    [ApiController]
    [Route("api/[controller]/")]
    [Produces("application/json")]
    [BindProperties]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ExcludeFromCodeCoverage]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionRepository _repository;
        private readonly ILogger<MessageController> _logger;

        /// <inheritdoc />
        public QuestionsController(
            IQuestionRepository repository,
            ILogger<MessageController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Add question to the list of the questions
        /// </summary>
        /// <param name="dto">Question to add
        /// {
        ///    "message": "What is your name",
        ///    "answer" : "A",
        ///    "point": 10,
        ///    "isPicture" : false
        /// }</param>
        /// <returns>True or false of the adding status</returns>
        /// <response code="200">Returns successfully status</response>
        /// <response code="400">Return bad handling status</response>
        [HttpPost]
        [ProducesResponseType(typeof(AddMessageStatusOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary),
            StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public ActionResult<AddMessageStatusOutDto> Add([FromBody] QuestionDto dto)
        {
            if (dto.IsPicture)
            {
                if (!Uri.IsWellFormedUriString(dto.Message, UriKind.Absolute))
                {
                    ModelState.AddModelError("message", "Url is invalid");
                }
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            var question = new Question
            {
                Answer = dto.Answer,
                Message = dto.Message,
                Point = dto.Point,
                IsPicture = dto.IsPicture
            };

            var result = _repository.Add(question);

            return result != null
                ? Ok(new
                {
                    Status = "successfully",
                    Question = question
                })
                : StatusCode(500, new {Status = "failed"});
        }

        /// <summary>
        /// Get all questions
        /// </summary>
        /// <returns>All questions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Question>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Question>> Get()
        {
            var result = _repository.All();
            return Ok(result);
        }

        /// <summary>
        /// Get all questions
        /// </summary>
        /// <returns>One question by id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<Question>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<Question>> Get([Required] string id)
        {
            ObjectId objectId;

            try
            {
                objectId = ObjectId.Parse(id);
            }
            catch (FormatException e)
            {
                return BadRequest(e.Message);
            }

            var result = _repository.Get(objectId);
            return Ok(result);
        }
    }
}