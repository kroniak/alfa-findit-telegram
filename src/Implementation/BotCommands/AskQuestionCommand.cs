using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using FindAlfaITBot.Models;
using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class AskQuestionCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;
        private Message _message;

        public AskQuestionCommand(TelegramBotClient botClient, long chatId, Message message)
        {
            _botClient = botClient ?? throw new Exception("BotClient is null");
            _chatId = chatId;
            _message = message;
        }

        public async void Execute()
        {
            var countResolvedQuestion = MongoDBHelperQuestion.GetResult(_chatId).Result.Questions.Count;
            var countAllQuestion = MongoDBHelperQuestion.AllQuestion().Result.Count();

            if (countResolvedQuestion == countAllQuestion)
            {
                var isEnd = MongoDBHelperQuestion.GetResult(_chatId).Result.isEnd;
                if (!isEnd)
                {
                    await _botClient.SendTextMessageAsync(_chatId, MessageFactory.EndMessage,
                        replyMarkup: BotHelper.GetRemoveKeyboard());
                    await MongoDBHelperQuestion.UpdateEnd(_chatId);
                    return;
                }
                return;
            }

            var question = MongoDBHelperQuestion.GetQuestion(countResolvedQuestion.ToString()).Result;

            var answer = _message.Text;

            if (IsAnswerValid(answer, question) == 0)
            {
                question.Point = 0;
            }

            await MongoDBHelperQuestion.SaveResultForUser(_chatId, question);
            var newCountResolvedQuestion = MongoDBHelperQuestion.GetResult(_chatId).Result.Questions.Count;

            if (newCountResolvedQuestion == countAllQuestion)
            {
                await MongoDBHelperQuestion.UpdatePoints(_chatId);
                var points = MongoDBHelperQuestion.GetResult(_chatId).Result.Points;
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.EndMessage
                    + "\nВаш результат: " + points,
                    replyMarkup: BotHelper.GetRemoveKeyboard());

                await MongoDBHelperQuestion.UpdateEnd(_chatId);
                return;
            }

            var nextQuestion = MongoDBHelperQuestion.GetQuestion(newCountResolvedQuestion.ToString()).Result;

            if (nextQuestion.IsPicture)
            {
                await _botClient.SendPhotoAsync(_chatId, new InputOnlineFile(nextQuestion.Message),
                        replyMarkup: BotHelper.GetKeyboardForQuestions());
            }
            else
            {
                await _botClient.SendTextMessageAsync(_chatId, nextQuestion.Message,
                        replyMarkup: BotHelper.GetKeyboardForQuestions());
            }
        }

        private int IsAnswerValid(string answer, Question question)
        {
            return string.Compare(answer, question.Answer);
        }
    }
}
