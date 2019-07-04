using System;
using System.Collections.Generic;
using System.Linq;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Factories.Dictionaries;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services.Helpers;
using AlfaBot.Core.Services.Interfaces;
using Telegram.Bot.Types;
using User = AlfaBot.Core.Models.User;

namespace AlfaBot.Core.Factories.Commands
{
    public class QuestionCommandFactory : IQuestionCommandFactory
    {
        private readonly IQueueService _queueService;
        private readonly IQuizResultRepository _resultRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;

        public QuestionCommandFactory(
            IQueueService queueService,
            IQuizResultRepository resultRepository,
            IQuestionRepository questionRepository,
            IUserRepository userRepository)
        {
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        private Action EndQuestionCommand(Message message)
        {
            return () =>
            {
                _queueService.Add(new QueueMessage(message.Chat.Id, message.MessageId, false)
                {
                    Text = QuizMessageDictionary.EndMessage,
                    ReplyMarkup = BotHelper.GetRemoveKeyboard()
                });
            };
        }

        public Action QuestionCommand(
            Message message,
            QueueMessage nextMessage,
            bool isAnsweredAll)
        {
            var chatId = message.Chat.Id;
            var result = _resultRepository.GetResult(chatId);
            // if quiz is not ended check answer
            var nonAnsweredQuestion = GetFirstNonAnswered(result.QuestionAnswers);
            if (nonAnsweredQuestion is null) return EndQuestionCommand(message);

            var question = _questionRepository.Get(nonAnsweredQuestion.QuestionId);

            var answer = message.Text;

            // update answer
            nonAnsweredQuestion.IsAnswered = true;
            nonAnsweredQuestion.Point = Compare(answer, question.Answer) ? question.Point : 0;
            nonAnsweredQuestion.Answer = answer;

            return () =>
            {
                // update db
                _resultRepository.UpdateQuestionsForUser(result);

                // check count
                var countAnswered = GetAnsweredCount(result.QuestionAnswers);
                var countAllQuestion = _questionRepository.Count;

                // if quiz is ended
                if (countAnswered == countAllQuestion)
                {
                    _userRepository.SetQuizMember(chatId, false);
                    _resultRepository.UpdateTimeForUser(result);

                    var appendix = "";
                    if (!isAnsweredAll)
                    {
                        appendix = "\n\nВам необходимо пройти опрос до конца. Осталось пару вопросов!";
                    }

                    _queueService.Add(new QueueMessage(chatId, message.MessageId)
                    {
                        Text = QuizMessageDictionary.EndMessage
                               + "\nТвой результат: " + result.Points + " баллов."
                               + appendix
                               + "\n\n" + nextMessage.Text,
                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
                    });
                }
                else
                {
                    HandleNewQuestion(result, message);
                }
            };
        }

        public Action AddQuizCommand(
            User user,
            Message message,
            QueueMessage nextFalseMessage,
            QueueMessage wrongMessage)
        {
            var answer = message.Text;

            return () =>
            {
                if (Compare(answer, "Викторина") || Compare(answer, "Да"))
                {
                    var result = _resultRepository.AddUserQuiz(user);

                    // generating answers from all questions
                    var answers = _questionRepository.All().Select(q => new QuestionAnswer
                    {
                        QuestionId = q.Id,
                        Point = 0,
                        IsAnswered = false
                    }).ToArray();

                    // random sort answers
                    var random = new Random();
                    var randomNumbers = answers.Select(r => random.Next()).ToArray();
                    var orderedAnswers = answers.Zip(randomNumbers, (r, o) => new {Result = r, Order = o})
                        .OrderBy(o => o.Order)
                        .Select(o => o.Result);

                    result.QuestionAnswers.AddRange(orderedAnswers);

                    _resultRepository.UpdateQuestionsForUser(result);
                    _userRepository.SetQuizMember(user.ChatId, true);

                    // add first question
                    HandleNewQuestion(result, message);
                }
                else if (Compare(answer, "Опрос") || Compare(answer, "Нет"))
                {
                    _userRepository.SetQuizMember(user.ChatId, false);
                    _queueService.Add(nextFalseMessage);
                }
                else
                {
                    _queueService.Add(wrongMessage);
                }
            };
        }

        private void HandleNewQuestion(QuizResult result, Message message)
        {
            var nextQuestionId = GetFirstNonAnswered(result.QuestionAnswers).QuestionId;
            var nextQuestion = _questionRepository.Get(nextQuestionId);

            if (nextQuestion.IsPicture)
            {
                _queueService.Add(new QueueMessage(message.Chat.Id, message.MessageId, false)
                {
                    Url = nextQuestion.Message,
                    ReplyMarkup = BotHelper.GetKeyboardForQuestions()
                });
            }
            else
            {
                _queueService.Add(new QueueMessage(message.Chat.Id, message.MessageId, false)
                {
                    Text = nextQuestion.Message,
                    ReplyMarkup = BotHelper.GetRemoveKeyboard()
                });
            }
        }

        private static int GetAnsweredCount(IEnumerable<QuestionAnswer> answers) =>
            answers.Count(q => q.IsAnswered);

        private static QuestionAnswer GetFirstNonAnswered(IEnumerable<QuestionAnswer> answers) =>
            answers.FirstOrDefault(q => !q.IsAnswered);

        private static bool Compare(string a, string b) =>
            string.Compare(a, b, StringComparison.InvariantCultureIgnoreCase) == 0;
    }
}