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

        public static IReplyMarkup GetKeyboardForCourse()
        {
            var junior = new KeyboardButton("1-3");
            var notJunior = new KeyboardButton("4 и старше");

            var keyboard = new[]
            {
                new[] {junior},
                new[] {notJunior}
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