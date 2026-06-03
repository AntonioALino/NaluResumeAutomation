using NaluResumeAutomation.Application.Abstractions;
using NaluResumeAutomation.Infra.Common;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace NaluResumeAutomation.Infra.ExternalServices.Telegram
{
    public class TelegramNotifier : BaseTelegramClient, ITelegramNotifier
    {
        public TelegramNotifier(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public async Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken)
        {
            await ExecuteSafeAsync(async () =>
            {
                await BotClient.SendMessage(
                    chatId: chatId,
                    text: message,
                    parseMode: ParseMode.Markdown, 
                    cancellationToken: cancellationToken
                );
            });
        }
        public async Task<Stream> DownloadFileAsync(string fileId, CancellationToken cancellationToken)
        {
            return await ExecuteSafeAsync(async () =>
            {
                var file = await BotClient.GetFile(fileId, cancellationToken);
                var memoryStream = new MemoryStream();
                await BotClient.DownloadFile(file.FilePath!, memoryStream, cancellationToken);
                memoryStream.Position = 0;

                return memoryStream;
            });
        }

    }
}
