using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AlfaBot.Host.Model
{
    /// <inheritdoc />
    public class TextMessageDto : MessageDto
    {
        /// <summary>
        /// For text messages, the actual UTF-8 text of the message
        /// </summary>
        [Required]
        public string Text { get; set; }
    }
}