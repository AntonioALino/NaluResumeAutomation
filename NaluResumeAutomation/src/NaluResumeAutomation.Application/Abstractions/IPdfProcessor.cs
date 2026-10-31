using NaluResumeAutomation.Application.useCases.Records.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaluResumeAutomation.Application.Abstractions
{
    public interface IPdfProcessor
    {
        Task<ProcessPdfResult> ProcessAsync(Stream pdfStream, CancellationToken cancellationToken);
    }
}
