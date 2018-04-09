namespace FindAlfaITBot.Factories
{
    public static class MessageFactory
    {
        public static string WelcomeMessage = "Привет! Добро пожаловать на стенд Альфа-Банка." +
            "\nВ нашем ИТ-подразделении работает более 2 000 человек: мы занимаемся поддержкой наших клиентов, разработкой и внедрением всех digital-продуктов."+
            "\nНаши продукты — это «Альфа-Клик», «Альфа-Мобайл», «Мобайл для бизнеса», «Поток»." +
            "\nДля начинающих специалистов мы готовы предложить стажировки и практики." +
            "\nОставь нам свой контакт из телефонной книги." +
            "\nОтправляя свои данные Вы соглашаетесь на обработку [персональных данных](https://goo.gl/em3CsJ) ";

        public static string AskNameMessage = "Контакт получен.\nКак мы сможем к тебе обращаться?";
        public static string AskContactMessage = "Пришлите свой контакт";
        public static string AskEmailMessage = "Теперь пришлите свой email.";
        public static string AskUniversityMessage = "Укажи свой ВУЗ и курс";
        public static string AskProfessionMessage = "Ты у цели!\nУкажи желаемое направление для развития или укажите свой";
        public static string EndOfAskingMessage = "Принято! Как только у нас появится подходящая вакансия, мы свяжемся с тобой.";
        public static string EndMessage = "Опрос завершен. Как только у нас появится подходящая вакансия, мы свяжемся с тобой.";
        public static string WrongEMailMessage = "Хмм, это не выглядит как email.\nПопробуйте ещё раз.";
    }
}