using NaluResumeAutomation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaluResumeAutomation.Domain.Entities
{
    public class StudyDocumentEntity : BaseEntity
    {
        public long ChatId { get; private set;}
        public string TelegramFileId { get; private set;}
        public string FileName { get; private set;}
        public DocumentStatus Status { get; private set;}
        public string? SummaryText { get; private set; }
        public string? MindMapMarkdown { get; private set; }

        public StudyDocumentEntity(long chatId, string telegramFileId, string fileName)
        {
            ChatId = chatId;
            TelegramFileId = telegramFileId;
            FileName = fileName;
            Status = DocumentStatus.Received;
        }
        public void MarkAsProcessing()
        {
            Status = DocumentStatus.Processing;
        }
        public void CompleteProcessing(string summary, string mindMap)
        {
            SummaryText = summary;
            MindMapMarkdown = mindMap;
            Status = DocumentStatus.Completed;
        }
        public void MarkAsFailed()
        {
            Status = DocumentStatus.Failed;
        }

    }
}
