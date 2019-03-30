//using System;
//using FindAlfaITBot.Factories;
//using FindAlfaITBot.Models;
//using FindAlfaITBot.Services.Interfaces;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.InputFiles;
//
//namespace FindAlfaITBot.Services.Commands
//{
//    public class AskQuestionCommand : IMessageCommand
//    {
//        private readonly TelegramBotClient _botClient;
//        private readonly long _chatId;
//        private readonly Message _message;
//
//        public AskQuestionCommand(TelegramBotClient botClient, long chatId, Message highPriorityMessage)
//        {
//            _botClient = botClient ?? throw new Exception("BotClient is null");
//            _chatId = chatId;
//            _message = highPriorityMessage;
//        }
//
//        public async void Execute()
//        {
//            var countResolvedQuestion = Question.GetResult(_chatId).Result.Questions.Count;
//            var countAllQuestion = Question.AllQuestion().Result.Count();
//
//            if (countResolvedQuestion == countAllQuestion)
//            {
//                var isEnd = Question.GetResult(_chatId).Result.isEnd;
//                if (!isEnd)
//                {
//                    await _botClient.SendTextMessageAsync(_chatId, MessageDictionary.EndMessage,
//                        replyMarkup: BotHelper.GetRemoveKeyboard());
//                    await Question.UpdateEnd(_chatId);
//                    return;
//                }
//                return;
//            }
//
//            var question = Question.GetQuestion(countResolvedQuestion.ToString()).Result;
//
//            var answer = _message.Text;
//
//            if (IsAnswerValid(answer, question) == 0)
//            {
//                question.Point = 0;
//            }
//
//            await Question.SaveResultForUser(_chatId, question);
//            var newCountResolvedQuestion = Question.GetResult(_chatId).Result.Questions.Count;
//
//            if (newCountResolvedQuestion == countAllQuestion)
//            {
//                await Question.UpdatePoints(_chatId);
//                var points = Question.GetResult(_chatId).Result.Points;
//                await _botClient.SendTextMessageAsync(_chatId, MessageDictionary.EndMessage
//                    + "\nВаш результат: " + points,
//                    replyMarkup: BotHelper.GetRemoveKeyboard());
//
//                await Question.UpdateEnd(_chatId);
//                return;
//            }
//
//            var nextQuestion = Question.GetQuestion(newCountResolvedQuestion.ToString()).Result;
//
//            if (nextQuestion.IsPicture)
//            {
//                await _botClient.SendPhotoAsync(_chatId, new InputOnlineFile(nextQuestion.Message),
//                        replyMarkup: BotHelper.GetKeyboardForQuestions());
//            }
//            else
//            {
//                await _botClient.SendTextMessageAsync(_chatId, nextQuestion.Message,
//                        replyMarkup: BotHelper.GetKeyboardForQuestions());
//            }
//        }
//
//        private int IsAnswerValid(string answer, Models.Question question)
//        {
//            return string.Compare(answer, question.Answer);
//        }
//    }
//}

