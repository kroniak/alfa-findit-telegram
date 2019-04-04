namespace AlfaBot.Host.Model
{
    /// <summary>
    /// DTO object for User controller
    /// </summary>
    public class UserOutDto
    {
        public long ChatId { get; set; }
        public string TelegramName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string University { get; set; }
        public string Profession { get; set; }
        public bool? IsStudent { get; set; }
        public string Course { get; set; }
    }
}