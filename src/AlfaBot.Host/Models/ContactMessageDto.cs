using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AlfaBot.Host.Models
{
    /// <inheritdoc />
    public class ContactMessageDto : MessageDto
    {
        /// <summary>Contact's phone number</summary>
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>Contact's first name</summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>Optional. Contact's last name</summary>
        [Required]
        public string LastName { get; set; }
    }
}