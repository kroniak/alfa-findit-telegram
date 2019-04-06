// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AlfaBot.Host.Model
{
    /// <summary>
    /// Out Dto about status of deleted
    /// </summary>
    public class UserDeletedOutDto
    {
        /// <summary>
        /// Count of user deleted
        /// </summary>
        public long UserDeletedCount { get; set; }

        /// <summary>
        /// Count of quiz deleted
        /// </summary>
        public long QuizDeletedCount { get; set; }
    }
}