// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;

namespace AlfaBot.Host.Models
{
    /// <summary>
    /// Question dto to API
    /// </summary>
    public class QuestionDto
    {
        /// <summary>
        /// Flag that question is picture
        /// </summary>
        [Required]
        public bool IsPicture { get; set; }
        
        /// <summary>
        /// Answer to the question at A B C D
        /// </summary>
        [Required]
        public string Answer { get; set; }
        
        /// <summary>
        /// Text of the question to user
        /// </summary>
        [Required]
        public string Message { get; set; }
        
        /// <summary>
        /// Point of the question to calculation results
        /// </summary>
        [Required]
        [Range(0, 100)]
        public double Point { get; set; }
    }
}