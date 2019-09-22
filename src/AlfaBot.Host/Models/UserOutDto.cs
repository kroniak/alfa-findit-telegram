// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AlfaBot.Host.Models
{
    /// <summary>
    /// DTO object for User controller
    /// </summary>
    public class UserOutDto
    {
        /// <summary>
        /// ChatId for User
        /// </summary>
        public long ChatId { get; set; }
        
        /// <summary>
        /// Telegram name
        /// </summary>
        public string TelegramName { get; set; }
        
        /// <summary>
        /// Correct name from answer
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Contact phone from contact telegram
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Email from answer
        /// </summary>
        public string EMail { get; set; }
        
        /// <summary>
        /// Bet from answer
        /// </summary>
        public string Bet { get; set; }
    }
}