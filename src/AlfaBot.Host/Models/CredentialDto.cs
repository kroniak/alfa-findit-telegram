using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AlfaBot.Host.Models
{
    /// <summary>
    /// User dto for Login procedure
    /// </summary>
    public class CredentialDto
    {
        /// <summary>
        /// UserName for login
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Not hashed password for login
        /// </summary>
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}