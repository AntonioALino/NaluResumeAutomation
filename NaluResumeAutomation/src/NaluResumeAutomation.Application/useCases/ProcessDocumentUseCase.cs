using NaluResumeAutomation.Application.Abstractions;
using NaluResumeAutomation.Application.Common;
using NaluResumeAutomation.Application.Common.Interfaces;
using NaluResumeAutomation.Application.useCases.Records.Request;
using NaluResumeAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaluResumeAutomation.Application.useCases
{
    public class ProcessDocumentUseCase : IUseCase<ProcessDocumentRequest, Result>
    {
        private readonly ITelegramNotifier _telegramNotifier;
        private readonly IPdfProcessor _pdfProcessor;
       public ProcessDocumentUseCase(ITelegramNotifier telegramNotifier, IPdfProcessor pdfProcessor)
        {
            _telegramNotifier = telegramNotifier;
            _pdfProcessor = pdfProcessor;
        }

       public async Task<Result> ExecuteAsync(ProcessDocumentRequest request, CancellationToken cancellationToken)
        {
            var document = new StudyDocumentEntity(request.ChatId, request.FileId, request.FileName);

            document.MarkAsProcessing();

            await _telegramNotifier.SendMessageAsync(
                request.ChatId,
                $"⏳ Recebi o arquivo *{request.FileName}*! Estou lendo e preparando seu resumo com mapa mental. Me dá uns minutinhos...", 
                cancellationToken);

            try
            {
                await using var pdfStream = await _telegramNotifier.DownloadFileAsync(request.FileId, cancellationToken);

                var aiResult = await _pdfProcessor.ProcessAsync(pdfStream, cancellationToken);

                document.CompleteProcessing(aiResult.SummaryText, aiResult.MindMapMarkdown);

                var finalMessage = $"✅ *Resumo Prontinho!*\n\n{document.SummaryText}\n\n" +
                               $"🗺️ *Mapa Mental (Markdown):*\n```mermaid\n{document.MindMapMarkdown}\n```";

                await _telegramNotifier.SendMessageAsync(request.ChatId, finalMessage, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                document.MarkAsFailed();

                await _telegramNotifier.SendMessageAsync(request.ChatId, "❌ Puxa, deu algum erro ao processar esse PDF. Pode tentar enviar de novo?", cancellationToken);

                return Result.Failure($"Falha ao processar o documento: {ex.Message}");
            }

        }

    }
}
