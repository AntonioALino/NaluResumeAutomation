using NaluResumeAutomation.Application.Abstractions;
using NaluResumeAutomation.Application.useCases.Records.Response;
using NaluResumeAutomation.Infra.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaluResumeAutomation.Infra.ExternalServices.APIs
{
    public class PythonPdfProcessor : BaseHttpClient, IPdfProcessor
    {
        public PythonPdfProcessor(HttpClient client) : base(client)
        {
        }

        public async Task<ProcessPdfResult> ProcessAsync(Stream pdfStream, CancellationToken cancellationToken)
        {
            var result = await PostFileAsync<ProcessPdfResult>(
            endpoint: "/api/process-pdf",
            fileStream: pdfStream,
            fileName: "documento.pdf",
            fileParameterName: "file",
            cancellationToken: cancellationToken
            );

            if (result is null)
            {
                throw new InvalidOperationException("A API do Python não retornou o formato esperado.");
            }

            return result;
        }
    }
}
