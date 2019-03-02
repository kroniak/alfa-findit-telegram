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
            var keyboardButton = new[] { new[] { button } };
            return new ReplyKeyboardMarkup { Keyboard = keyboardButton };
        }

        public static ReplyKeyboardRemove GetRemoveKeyboard() => new ReplyKeyboardRemove { Selective = true };

        public static ReplyKeyboardMarkup GetKeyboardYesOrNo()
        {
            var yesButton = new KeyboardButton("Да");
            var noButton = new KeyboardButton("Нет");

            var keyboard = new[]
            {
                new[] {yesButton},
                new[] {noButton}
            };
            return new ReplyKeyboardMarkup { Keyboard = keyboard };
        }

        public static ReplyKeyboardMarkup GetKeyboardForCourse()
        {
            var junior = new KeyboardButton("1-3");
            var notJunior = new KeyboardButton("4 и старше");

            var keyboard = new[]
            {
                new[] {junior},
                new[] {notJunior}
            };
            return new ReplyKeyboardMarkup { Keyboard = keyboard };
        }

        public static ReplyKeyboardMarkup GetKeyboardForQuestions()
        {
            var aButton = new KeyboardButton("A");
            var bButton = new KeyboardButton("B");
            var cButton = new KeyboardButton("C");
            var dButton = new KeyboardButton("D");

            var keyboard = new[]
            {
                new[]
                {
                    aButton,
                    bButton
                },
                new[]
                {
                    cButton,
                    dButton
                }
            };
            return new ReplyKeyboardMarkup { Keyboard = keyboard };
        }

        public static ReplyKeyboardMarkup GetKeyboardForProfession()
        {
            var dotnetButton = new KeyboardButton("Разработка .NET");
            var javaButton = new KeyboardButton("Разработка Java");
            var javaScriptButton = new KeyboardButton("Разработка JavaScript");
            var analyticsButton = new KeyboardButton("Системная аналитика");
            var supportButton = new KeyboardButton("Системное сопровождение");
            var testingButton = new KeyboardButton("Автотестирование");


            var keyboard = new[]
            {
                new[] {javaButton}, new[] {analyticsButton},
                new[] {supportButton}, new[] {testingButton},
                new[] {dotnetButton}, new[] {javaScriptButton}
            };
            return new ReplyKeyboardMarkup { Keyboard = keyboard };
        }
    }
}