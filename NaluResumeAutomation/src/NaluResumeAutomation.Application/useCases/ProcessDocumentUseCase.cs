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

                var summaryMessage = $"✅ *Resumo Prontinho!*\n\n{document.SummaryText}";
                await _telegramNotifier.SendMessageAsync(request.ChatId, summaryMessage, cancellationToken);
                var mindMapMessage = $"✅ *Mapa Mental Prontinho!*\n\n{document.MindMapMarkdown}";
                await _telegramNotifier.SendMessageAsync(request.ChatId, mindMapMessage, cancellationToken);

                try
                {
                    var content = new StringContent(document.MindMapMarkdown, System.Text.Encoding.UTF8, "text/plain");

                    using var httpClient = new HttpClient();
                    var chartResponse = await httpClient.PostAsync("https://kroki.io/mermaid/png", content, cancellationToken);

                    if (chartResponse.IsSuccessStatusCode)
                    {
                        await using var imageStream = await chartResponse.Content.ReadAsStreamAsync(cancellationToken);
                        await _telegramNotifier.SendPhotoAsync(
                            request.ChatId,
                            imageStream,
                            "🗺️ *Aqui está o seu Mapa Mental!*",
                            cancellationToken);
                    }
                    else
                    {
                        var erroApi = await chartResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"[ERRO KROKI]: {erroApi}");
                        await _telegramNotifier.SendMessageAsync(request.ChatId, $"🗺️ *Mapa Mental (Texto):*\n\n{document.MindMapMarkdown}", cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERRO DE REDE - IMAGEM]: {ex.Message}");
                    await _telegramNotifier.SendMessageAsync(request.ChatId, $"🗺️ *Mapa Mental (Texto):*\n\n{document.MindMapMarkdown}", cancellationToken);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                document.MarkAsFailed();

                Console.WriteLine($"[ERRO CRÍTICO NO USE CASE]: {ex}");

                await _telegramNotifier.SendMessageAsync(request.ChatId, "❌ Puxa, deu algum erro ao processar esse PDF. Pode tentar enviar de novo?", cancellationToken);

                return Result.Failure($"Falha ao processar o documento: {ex.Message}");
            }

        }

    }
}
