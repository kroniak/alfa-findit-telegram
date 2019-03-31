using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlfaBot.Core.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

// ReSharper disable ClassNeverInstantiated.Global

namespace AlfaBot.Host.Middleware
{
    /// <summary>
    /// Background task service for sending data to Telegram
    /// </summary>
    [ExcludeFromCodeCoverage]
    // ReSharper disable once InheritDocConsiderUsage
    public class SendingHostedService : IHostedService, IDisposable
    {
        private const int Limit = 30;
        private readonly ILogger<SendingHostedService> _logger;
        private readonly IQueueService _queueService;
        private readonly ITelegramBotClient _client;
        private Timer _timer;

        /// <inheritdoc />
        public SendingHostedService(
            ILogger<SendingHostedService> logger,
            IQueueService queueService,
            ITelegramBotClient client
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Sending Background Service is starting.");

            _timer = new Timer(SendMessages, new { }, TimeSpan.Zero, TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private void SendMessages(object o)
        {
            lock (o)
            {
                _logger.LogDebug("Send Background Service is started process.");

                var messages = _queueService.GetTopHighPriority(Limit).ToList();

                if (!messages.Any())
                {
                    _logger.LogDebug("Send Background Service is successfully with 0 count");
                    return;
                }

                if (messages.Count < Limit)
                {
                    messages.AddRange(_queueService.GetTopLowPriority(Limit - messages.Count));
                }

                Parallel.ForEach(messages, message =>
                {
                    try
                    {
                        _client.SendTextMessageAsync(
                                message.ChatId,
                                message.Text,
                                replyMarkup: message.ReplyMarkup)
                            .GetAwaiter().GetResult();

                        _queueService.Dequeue(message.Id);

                        _logger.LogInformation($"[{message.ChatId}] [{message.Text ?? ""}] Sending successfully");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"[{message.ChatId}] [{message.Text ?? ""}] Sending failed", e);
                    }
                });

                _logger.LogDebug("Send Background Service is successfully");
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Sending Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose() => _timer?.Dispose();
    }
}