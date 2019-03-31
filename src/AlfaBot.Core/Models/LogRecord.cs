using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlfaBot.Core.Models
{
    /// <inheritdoc />
    public class LogRecord
    {
        public LogRecord(Message message)
        {
            MessageId = message.MessageId;
            ChatId = message.Chat.Id;
            Text = message.Text;
            Contact = message.Contact;
            Type = message.Type;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        
        /// <summary>Unique message identifier</summary>
        public int MessageId { get; set; }
        
        /// <summary>
        /// Gets the <see cref="T:Telegram.Bot.Types.Enums.MessageType" /> of the <see cref="T:Telegram.Bot.Types.Message" />
        /// </summary>
        /// <value>
        /// The <see cref="T:Telegram.Bot.Types.Enums.MessageType" /> of the <see cref="T:Telegram.Bot.Types.Message" />
        /// </value>
        public MessageType Type { get; set; }
        
        /// <summary>Conversation the message belongs to</summary>
        public long ChatId { get; set; }
        
        /// <summary>
        /// Optional. Description is a shared contact, information about the contact
        /// </summary>
        public Contact Contact { get; set; }
        
        /// <summary>
        /// Optional. For text messages, the actual UTF-8 text of the message
        /// </summary>
        public string Text { get; set; }
        
        public QueueMessage QueueMessage { get; set; }

        public DateTime Start { get; set; } = DateTime.Now;

        public DateTime? End { get; set; }
    }
}