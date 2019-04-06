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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [BindProperties]
    [ExcludeFromCodeCoverage]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IQuizResultRepository _resultRepository;
        private readonly ILogger<UsersController> _logger;

        /// <inheritdoc />
        public UsersController(
            IUserRepository userRepository,
            IQuizResultRepository resultRepository,
            ILogger<UsersController> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all user information with json and csv result
        /// </summary>
        /// <returns>All user information with json and csv result</returns>
        [HttpGet]
        [Produces("application/json", "text/csv")]
        [ProducesResponseType(typeof(IEnumerable<UserOutDto>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<UserOutDto>> Get()
        {
            var result = Map(_userRepository.All());

            return Ok(result);
        }

        /// <summary>
        /// Get one user information with json result
        /// </summary>
        /// <param name="chatId">ChatId long number</param>
        /// <returns>One user information with json result</returns>
        [HttpGet("{chatId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<UserOutDto> Get([Required] [Range(1, long.MaxValue)] long chatId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            User user;
            try
            {
                user = _userRepository.Get(chatId);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (user != null)
            {
                return Ok(MapOne(user));
            }

            return NotFound();
        }

        /// <summary>
        /// Delete one user information and Quiz result permanently
        /// </summary>
        /// <param name="chatId">ChatId long number</param>
        /// <returns>Information about delete status</returns>
        [HttpDelete("{chatId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserDeletedOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<UserDeletedOutDto> Delete([Required] [Range(1, long.MaxValue)] long chatId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("This model is invalid.", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var quizDeleteResult = _resultRepository.Delete(chatId);
                var userDeleteResult = _userRepository.Delete(chatId);

                var result = new UserDeletedOutDto
                {
                    QuizDeletedCount = quizDeleteResult.DeletedCount,
                    UserDeletedCount = userDeleteResult.DeletedCount
                };

                if (result.UserDeletedCount == 0)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        private static IEnumerable<UserOutDto> Map(IEnumerable<User> users) => users.Select(MapOne).ToArray();

        private static UserOutDto MapOne(User user) =>
            new UserOutDto
            {
                Name = user.Name,
                ChatId = user.ChatId,
                Phone = user.Phone,
                Course = user.Course,
                Profession = user.Profession,
                University = user.University,
                EMail = user.EMail,
                TelegramName = user.TelegramName
            };
    }
}