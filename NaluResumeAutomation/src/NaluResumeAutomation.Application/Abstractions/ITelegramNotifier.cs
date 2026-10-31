using System;
using System.Collections.Generic;
using System.Text;

namespace NaluResumeAutomation.Application.Abstractions;

public interface ITelegramNotifier
{
    Task SendMessageAsync(long chatId, string? message, CancellationToken cancellationToken);
    Task<Stream> DownloadFileAsync(string fileId, CancellationToken cancellationToken);
    Task SendPhotoAsync(long chatId, Stream photoStream, string caption, CancellationToken cancellationToken);
}

