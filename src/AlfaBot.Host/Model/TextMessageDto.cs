using System.ComponentModel.DataAnnotations;

namespace AlfaBot.Host.Model
{
    /// <summary>
    /// Text message from client
    /// </summary>
    public class TextMessageDto : MessageDto
    {
        /// <summary>
        /// For text messages, the actual UTF-8 text of the message
        /// </summary>
        [Required]
        public string Text { get; set; }
    }
}