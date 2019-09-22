using Telegram.Bot.Types.ReplyMarkups;

namespace AlfaBot.Core.Services.Helpers
{
    public static class BotHelper
    {
        private const string ContactButtonText = "Нажми сюда, чтобы дать свой контакт";

        public static IReplyMarkup GetKeyBoardForContact()
        {
            var button = new KeyboardButton("contact")
            {
                RequestContact = true,
                Text = ContactButtonText
            };
            var keyboardButton = new[] {new[] {button}};
            return new ReplyKeyboardMarkup {Keyboard = keyboardButton};
        }

        public static IReplyMarkup GetRemoveKeyboard() => new ReplyKeyboardRemove {Selective = true};

        public static IReplyMarkup GetKeyboardForBet()
        {
            var keyboard = new[]
            {
                new[] {new KeyboardButton("Молчанов Николай - Мутационное тестирование")},
                new[] {new KeyboardButton("ХХХ - ХХХ")},
                new[] {new KeyboardButton("Кто то - какой то доклад")}
            };
            return new ReplyKeyboardMarkup {Keyboard = keyboard};
        }

        public static IReplyMarkup GetKeyboardForName(string telegramName)
        {
            var nameButton = new KeyboardButton(telegramName);

            var keyboard = new[]
            {
                new[] {nameButton}
            };

            return new ReplyKeyboardMarkup {Keyboard = keyboard};
        }
    }
}