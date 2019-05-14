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

        public static IReplyMarkup GetKeyboardYesOrNo()
        {
            var yesButton = new KeyboardButton("Да");
            var noButton = new KeyboardButton("Нет");

            var keyboard = new[]
            {
                new[] {yesButton},
                new[] {noButton}
            };
            return new ReplyKeyboardMarkup {Keyboard = keyboard};
        }

        public static IReplyMarkup GetKeyboardQuizOrNot()
        {
            var yesButton = new KeyboardButton("Викторина");
            var noButton = new KeyboardButton("Опрос");

            var keyboard = new[]
            {
                new[] {yesButton},
                new[] {noButton}
            };
            return new ReplyKeyboardMarkup {Keyboard = keyboard};
        }

        public static IReplyMarkup GetKeyboardForQuestions()
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
            return new ReplyKeyboardMarkup {Keyboard = keyboard};
        }

        public static IReplyMarkup GetKeyboardForProfession()
        {
            var dotnetButton = new KeyboardButton("Разработка на SQL");
            var javaButton = new KeyboardButton("Разработка на .NET Core");
            var javaScriptButton = new KeyboardButton("Разработка на MVC .NET");
            var analyticsButton = new KeyboardButton("Разработка сервисов WCF");
            var supportButton = new KeyboardButton("Fullstack разработка .NET + JS");
            var testingButton = new KeyboardButton("Автотестирование");


            var keyboard = new[]
            {
                new[] {javaButton}, new[] {analyticsButton},
                new[] {supportButton}, new[] {testingButton},
                new[] {dotnetButton}, new[] {javaScriptButton}
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