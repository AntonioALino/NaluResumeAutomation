using NaluResumeAutomation.Application.Abstractions;
using NaluResumeAutomation.Infra.Common;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
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
                try
                {
                    await BotClient.SendMessage(
                        chatId: chatId,
                        text: message,
                        parseMode: ParseMode.Markdown,
                        cancellationToken: cancellationToken
                    );
                }
                catch (ApiRequestException)
                {
                    await BotClient.SendMessage(
                        chatId: chatId,
                        text: message,
                        cancellationToken: cancellationToken
                    );
                }
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

        public async Task SendPhotoAsync(long chatId, Stream photoStream, string caption, CancellationToken cancellationToken)
        {
            await ExecuteSafeAsync(async () =>
            {
                await BotClient.SendPhoto(
                    chatId: chatId,
                    photo: InputFile.FromStream(photoStream, "mapamental.png"),
                    caption: caption,
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );
            });
        }
    }
}
