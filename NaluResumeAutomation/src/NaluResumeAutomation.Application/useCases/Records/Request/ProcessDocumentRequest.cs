using System;
using System.Collections.Generic;
using System.Text;

namespace NaluResumeAutomation.Application.useCases.Records.Request
{
    public record ProcessDocumentRequest(long ChatId, string FileId, string FileName)
    {
        
    }
}
