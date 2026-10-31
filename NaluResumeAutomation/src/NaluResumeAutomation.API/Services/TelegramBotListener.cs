using NaluResumeAutomation.Application.useCases;
using NaluResumeAutomation.Application.useCases.Records.Request;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NaluResumeAutomation.Worker.BackgroundServices
{
    public class TelegramBotListener : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TelegramBotListener> _logger;

        public TelegramBotListener(
            ITelegramBotClient botClient,
            IServiceScopeFactory scopeFactory,
            ILogger<TelegramBotListener> logger)
        {
            _botClient = botClient;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Iniciando o robô do Telegram...");
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
            );

            var me = await _botClient.GetMe(stoppingToken);
            _logger.LogInformation($"🤖 Bot @{me.Username} iniciado e escutando mensagens...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { Document: { } document } message)
                return;

            if (!document.FileName!.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                await botClient.SendMessage(message.Chat.Id, "⚠️ Por favor, envie apenas arquivos em formato PDF.", cancellationToken: cancellationToken);
                return;
            }

            _logger.LogInformation($"Recebido PDF: {document.FileName} do chat {message.Chat.Id}");

            using var scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<ProcessDocumentUseCase>();

            var request = new ProcessDocumentRequest(message.Chat.Id, document.FileId, document.FileName);

            await useCase.ExecuteAsync(request, cancellationToken);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Erro na API de Polling do Telegram");
            return Task.CompletedTask;
        }
    }
}
