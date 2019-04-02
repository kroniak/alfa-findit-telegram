using System;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Factories;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services.Interfaces;
using Telegram.Bot.Types;
using User = AlfaBot.Core.Models.User;

namespace AlfaBot.Core.Services.Commands
{
    public class QuestionCommandFactory : IQuestionCommandFactory
    {
        private readonly IQueueService _queueService;
        private readonly IQuizResultRepository _resultRepository;
//        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;

        public QuestionCommandFactory(
            IQueueService queueService,
            IQuizResultRepository resultRepository,
//            IQuestionRepository questionRepository,
            IUserRepository userRepository)
        {
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
//            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public Action EndQuestionCommand(long chatId, int messageId)
        {
            return () =>
            {
                _queueService.Add(new QueueMessage(chatId, messageId, false)
                {
                    Text = QuizMessageDictionary.EndMessage,
                    ReplyMarkup = BotHelper.GetRemoveKeyboard()
                });
            };
        }

//        public Action QuestionCommand(QuizResult result)
//        {
//            var countResolvedQuestion = result.QuestionAnswers.Count;
//            var countAllQuestion = _questionRepository.Count;
//
//            // check count of answered questions and total count
//            if (countResolvedQuestion == countAllQuestion)
//            {
//                var isEnd = result.isEnd;
//                if (!isEnd)
//                {
//                    return () =>
//                    {
//                        _resultRepository.UpdateEnd(chatId);
//
//                        _queueService.Add(new QueueMessage(chatId, false)
//                        {
//                            Text = QuizMessageDictionary.EndMessage,
//                            ReplyMarkup = BotHelper.GetRemoveKeyboard()
//                        });
//                    };
//                }
//            }
//
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
//                await _botClient.SendTextMessageAsync(_chatId, GeneralMessageDictionary.EndMessage
//                                                               + "\nВаш результат: " + points,
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
//                    replyMarkup: BotHelper.GetKeyboardForQuestions());
//            }
//            else
//            {
//                await _botClient.SendTextMessageAsync(_chatId, nextQuestion.Message,
//                    replyMarkup: BotHelper.GetKeyboardForQuestions());
//            }
//        }

        public Action AddQuizCommand(User user, Message message)
        {
            var answer = message.Text;

            if (string.Compare(answer, "да", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return () =>
                {
                    _resultRepository.AddUserQuiz(user);
                    _userRepository.SetQuizMember(user.ChatId, true);

                    // TODO add first question
//                    _queueService.Add(new QueueMessage(user.ChatId)
//                    {
//                        Text = QuizMessageDictionary.EndMessage,
//                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
//                    });
                };
            }

            if (string.Compare(answer, "нет", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return () =>
                {
                    _userRepository.SetQuizMember(user.ChatId, false);
                    _queueService.Add(new QueueMessage(user.ChatId, message.MessageId)
                    {
                        Text = GeneralMessageDictionary.EmailMessage,
                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
                    });
                };
            }

            return () =>
            {
                _queueService.Add(new QueueMessage(user.ChatId, message.MessageId)
                {
                    Text = QuizMessageDictionary.StartMessage,
                    ReplyMarkup = BotHelper.GetKeyboardYesOrNo()
                });
            };
        }

//        private int IsAnswerValid(string answer, Models.Question question)
//        {
//            return string.Compare(answer, question.Answer);
//        }
    }
}