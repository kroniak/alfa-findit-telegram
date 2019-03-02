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
            _botClient = botClient ?? throw new Exception();
            _chatId = chatId;
            _message = message;
        }

        public async void Execute()
        {
            var countResolvedQuestion = MongoDBHelper.GetResult(_chatId).Result.Questions.Count;
            var countAllQuestion = MongoDBHelper.AllQuestion().Result.Count();

            if (countResolvedQuestion == countAllQuestion)
            {
                var isEnd = MongoDBHelper.GetResult(_chatId).Result.isEnd;
                if (!isEnd)
                {
                    await _botClient.SendTextMessageAsync(_chatId, MessageFactory.EndMessage,
                        replyMarkup: BotHelper.GetRemoveKeyboard());
                    await MongoDBHelper.UpdateEnd(_chatId);
                    return;
                }
                else
                {
                    return;
                }
            }

            var question = MongoDBHelper.GetQuestion(countResolvedQuestion.ToString()).Result;

            var answer = _message.Text;

            if (!IsAnswerValid(answer, question))
            {
                question.Point = 0;
            }

            await MongoDBHelper.SaveResultForUser(_chatId, question);
            var newCountResolvedQuestion = MongoDBHelper.GetResult(_chatId).Result.Questions.Count;

            if (newCountResolvedQuestion == countAllQuestion)
            {
                await MongoDBHelper.UpdatePoints(_chatId);
                var points = MongoDBHelper.GetResult(_chatId).Result.Points;
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.EndMessage
                    + "\nВаш результат: " + points,
                    replyMarkup: BotHelper.GetRemoveKeyboard());

                await MongoDBHelper.UpdateEnd(_chatId);
                return;
            }

            var nextQuestion = MongoDBHelper.GetQuestion(newCountResolvedQuestion.ToString()).Result;

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

        private bool IsAnswerValid(string answer, Question question)
        {
            return Convert.ToBoolean(string.Compare(answer, question.Answer));
        }
    }
}
