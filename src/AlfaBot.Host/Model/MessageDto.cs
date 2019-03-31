using System.ComponentModel.DataAnnotations;

namespace AlfaBot.Host.Model
{
    /// <summary>
    /// Abstract messageDTO fot testing client
    /// </summary>
    public abstract class MessageDto
    {
        /// <summary>Conversation the message belongs to</summary>
        [Required]
        [Range(1, long.MaxValue)]
        public long ChatId { get; set; }
    }
}