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

        public static ReplyKeyboardRemove GetRemoveKeyboard()
        {
            return new ReplyKeyboardRemove { RemoveKeyboard = true };
        }
        
        public static ReplyKeyboardMarkup GetKeyboardYesOrNo()
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
        
        public static ReplyKeyboardMarkup GetKeyboardForCourse()
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

        public static ReplyKeyboardMarkup GetKeyboardForProfession()
        {
            var javaButton = new KeyboardButton("Разработка Java");
            var abcButton = new KeyboardButton("Разработка АБС (Автоматизированной банковской системы)");
            var bpmButton = new KeyboardButton("Разработка BPM системы (Управление бизнес-процессами)");
            var analyticsButton = new KeyboardButton("Системная аналитика");
            var supportButton = new KeyboardButton("Системное сопровождение");
            var devOpsButton = new KeyboardButton("Автоматизация тестирования (DevOps)");
            var testingButton = new KeyboardButton("Тестирование");
            var testingABCButton = new KeyboardButton("Тестирование АБС");
            var dotnetButton = new KeyboardButton("Разработка .NET");
            var creditButton = new KeyboardButton("Кредитный специалист для работы с физическими лицами");
            var KKOButton = new KeyboardButton("Стажер в ККО со знанием английского языка (на июнь 2018 г)");


            var keyboard = new[]
            {
                new[] {javaButton}, new[] {abcButton}, new[] {bpmButton}, new[] {analyticsButton},
                new[] {supportButton}, new[] {devOpsButton}, new[] {testingButton}, new[] {testingABCButton},
                new[] {dotnetButton}, new[] {creditButton}, new[] {KKOButton}
            };
            return new ReplyKeyboardMarkup { Keyboard = keyboard };
        }
    }
}