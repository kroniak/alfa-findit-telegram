using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FindAlfaITBot.Infrastructure
{
    public static class BotHelper
    {
        private const string ContactButtonText = "Нажми сюда, чтобы дать свой контакт";

        public static ReplyKeyboardMarkup GetKeyBoardForContact()
        {
            var button = new KeyboardButton("contact")
            {
                RequestContact = true,
                Text = ContactButtonText
            };
            var keyboardButton = new[] {new[] {button}};
            return new ReplyKeyboardMarkup {Keyboard = keyboardButton};
        }

        public static ReplyKeyboardRemove GetRemoveKeyboard()
        {
            return new ReplyKeyboardRemove {RemoveKeyboard = true};
        }

        public static ReplyKeyboardMarkup GetKeyboardForProfession()
        {
            var javaButton = new KeyboardButton("Java");
            var javaScriptButton = new KeyboardButton("JavaScript");
            var analyticsButton = new KeyboardButton("Аналитика");
            var testingButton = new KeyboardButton("Тестирование");
            var dotnetButton = new KeyboardButton(".NET");

            var keyboard = new[]
            {
                new[] {javaButton}, new[] {javaScriptButton}, new[] {analyticsButton}, new[] {testingButton},
                new[] {dotnetButton}
            };
            return new ReplyKeyboardMarkup {Keyboard = keyboard};
        }
    }
}