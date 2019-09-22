using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using AlfaBot.Host.Models;
using AlfaBot.Host.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace AlfaBot.Host.Controllers
{
    /// <inheritdoc />
    [Authorize(Roles = "Administrators,Users")]
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [BindProperties]
    [ExcludeFromCodeCoverage]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        /// <inheritdoc />
        public UsersController(
            IUserRepository userRepository,
            ILogger<UsersController> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all user information with json and csv result
        /// </summary>
        /// <returns>All user information with json and csv result</returns>
        [HttpGet]
        [Produces("application/json", "text/csv")]
        [ProducesResponseType(typeof(IEnumerable<UserOutDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UserOutDto>> Get()
        {
            IEnumerable<UserOutDto> result;

            if (HttpContext.User.IsInRole("Administrators"))
            {
                result = Map(_userRepository.All());
            }
            else if (HttpContext.User.IsInRole("Users"))
            {
                result = Map(_userRepository.All(), true);
            }
            else
            {
                result = new UserOutDto[] { };
            }

            return Ok(result);
        }

        /// <summary>
        /// Get one user information with json result
        /// </summary>
        /// <param name="chatId">ChatId long number</param>
        /// <returns>One user information with json result</returns>
        [Authorize(Roles = "Administrators")]
        [HttpGet("{chatId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary),
            StatusCodes.Status400BadRequest)]
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
        [Authorize(Roles = "Administrators")]
        [HttpDelete("{chatId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserDeletedOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), StatusCodes.Status400BadRequest)]
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
                var userDeleteResult = _userRepository.Delete(chatId);

                var result = new UserDeletedOutDto
                {
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

        private static IEnumerable<UserOutDto> Map(IEnumerable<User> users, bool mask = false) =>
            users.Select(u => MapOne(u, mask)).ToArray();

        private static UserOutDto MapOne(User user, bool mask = false) =>
            new UserOutDto
            {
                Name = user.Name,
                ChatId = mask ? 0 : user.ChatId,
                Phone = mask ? user.Phone.MaskMobile(3, "****") : user.Phone,
                EMail = user.EMail,
                Bet = user.Bet,
                TelegramName = user.TelegramName
            };
    }
}