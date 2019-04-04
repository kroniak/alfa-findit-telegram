using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using AlfaBot.Core.Data.Interfaces;
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [BindProperties]
    [ExcludeFromCodeCoverage]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        /// <inheritdoc />
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Get all user information with json result
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<UserOutDto>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<UserOutDto>> Json()
        {
            var result = _userRepository.All().Select(u => new UserOutDto
            {
                Name = u.Name,
                ChatId = u.ChatId,
                Phone = u.Phone,
                Course = u.Course,
                Profession = u.Profession,
                University = u.University,
                EMail = u.EMail,
                IsStudent = u.IsStudent ?? false,
                TelegramName = u.TelegramName
            });

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("text/plain")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public IActionResult Csv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("ChatId;EMail;Name;Profession;University;Course;Phone;Telegram;");

            var users = _userRepository.All();

            foreach (var p in users)
            {
                sb.AppendLine(
                    $"{p.ChatId}{p.EMail};{p.Name};{p.Profession};{p.University};{p.Course};{p.Phone};{p.TelegramName};");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "data.csv");
        }
    }
}