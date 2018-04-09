using System;
using System.Linq;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Models;
using Telegram.Bot;

namespace FindAlfaITBot
{
    class Program
    {
        static int Main(string[] args)
        {
            var date = DateTime.Now;
            Console.WriteLine($"Hello, I'm a bot! Today is {date.DayOfWeek}, it's {date:HH:mm} now.");

            // Config
            var token = Environment.GetEnvironmentVariable("FINDIT_BOT_TOKEN");
            var connection = Environment.GetEnvironmentVariable("FINDIT_MONGO");

            if (!string.IsNullOrEmpty(connection))
                MongoDBHelper.ConfigureConnection(connection);

            TestDB();

            if (!string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Telegram token must be not null");

            Console.WriteLine($"Connection string is {MongoDBHelper.GetConnectionName}");

            var bot = new FindITBot(token);
            bot.Build().Start();

            Console.ReadLine();

            return 0;
        }
        private static void TestDB()
        {
            var student = new Student
            {
                Id = Guid.NewGuid().ToString(),
                EMail = "test@test.ru",
                Name = "Test Student",
                Profession = "Haskell",
                University = "sgu"
            };

            MongoDBHelper.AddStudent(student);

            var collection = MongoDBHelper.All().Result;

            if (collection.Count() == 0)
                throw new Exception("Fail to start DB");

            foreach (var stud in collection)
            {
                Console.WriteLine($"Id = {stud.Id}");
                Console.WriteLine($"Email = {stud.EMail}");
                Console.WriteLine($"Name = {stud.Name}");
                Console.WriteLine($"Proff = {stud.Profession}");
                Console.WriteLine($"Univer = {stud.University}");
            }

        }
    }
}