using System;
using FindAlfaITBot.Infrastructure;
using Telegram.Bot;

namespace FindAlfaITBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hi! I am bot");

            // var student = new Student
            // {
            //     Id = Guid.NewGuid().ToString(),
            //     EMail = "123@123.ru",
            //     Name = "Name2",
            //     Profession = "nnnnnoooo",
            //     University = "sgu"
            // };
            // MongoDBHelper.AddStudent(student);
            // var collection = MongoDBHelper.All().Result;
            // foreach (var stud in collection)
            // {
            //     Console.WriteLine($"Id = {stud.Id}");
            //     Console.WriteLine($"Email = {stud.EMail}");
            //     Console.WriteLine($"Name = {stud.Name}");
            //     Console.WriteLine($"Proff = {stud.Profession}");
            //     Console.WriteLine($"Univer = {stud.University}");
            // }

            // MongoDBHelper.ConfigureConnection("mongodb://docker");
            
            new FindITBot("590696858:AAFtdRoDRffsMwOab4Lv7MwlYxcUGJS1n0w").Build().Start();
            Console.ReadLine();
        }
    }
}