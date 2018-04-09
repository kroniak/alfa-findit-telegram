namespace FindAlfaITBot.Interfaces
{
    public interface ITelegramBot
    {
        ITelegramBot Start();
        ITelegramBot Stop();
        bool Ping();

        string SecretKey { get; }
    }
}