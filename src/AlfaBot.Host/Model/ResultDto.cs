namespace AlfaBot.Host.Model
{
    /// <summary>
    /// Result class for exporting and view in JS
    /// </summary>
    public class ResultDto
    {
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
        /// Total points
        /// </summary>
        public double Points { get; set; }

        /// <summary>
        /// Total seconds
        /// </summary>
        public int Seconds { get; set; }
    }
}