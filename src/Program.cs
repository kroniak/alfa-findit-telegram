using System;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Models;
using Telegram.Bot;

namespace FindAlfaITBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var date = DateTime.Now;
            Console.WriteLine($"Hello, I'm a bot! Today is {date.DayOfWeek}, it's {date:HH:mm} now.");

            // MongoDBHelper.ConfigureConnection("mongodb://docker"); // for testing on docker

            Console.WriteLine($"Connection string is {MongoDBHelper.GetConnectionName}");

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

            foreach (var stud in collection)
            {
                Console.WriteLine($"Id = {stud.Id}");
                Console.WriteLine($"Email = {stud.EMail}");
                Console.WriteLine($"Name = {stud.Name}");
                Console.WriteLine($"Proff = {stud.Profession}");
                Console.WriteLine($"Univer = {stud.University}");
            }

            new FindITBot("590696858:AAFtdRoDRffsMwOab4Lv7MwlYxcUGJS1n0w").Build().Start();
            Console.ReadLine();
        }
    }
}