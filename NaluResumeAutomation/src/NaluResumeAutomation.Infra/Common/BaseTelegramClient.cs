using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace NaluResumeAutomation.Infra.Common
{
    public abstract class BaseTelegramClient
    {
        protected readonly ITelegramBotClient BotClient;

        protected BaseTelegramClient(ITelegramBotClient botClient)
        {
            BotClient = botClient;
        }

        protected async Task<T> ExecuteSafeAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (ApiRequestException ex)
            {
                throw;
            }
        }

        protected async Task ExecuteSafeAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (ApiRequestException ex)
            {
                throw;
            }
        }

    }
}
