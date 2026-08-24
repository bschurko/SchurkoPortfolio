namespace SchurkoPortfolio.Core.Model
{
    public class EmailVectorRecord
    {
        [Microsoft.Extensions.VectorData.VectorStoreKey]
        public int Id { get; set; }
        [Microsoft.Extensions.VectorData.VectorStoreData]
        public string? Email { get; set; } = string.Empty;
        [Microsoft.Extensions.VectorData.VectorStoreData]
        public string? Subject { get; set; } = string.Empty;
        [Microsoft.Extensions.VectorData.VectorStoreData]
        public string? Message { get; set; } = string.Empty;
        [Microsoft.Extensions.VectorData.VectorStoreData]
        public DateTime? DateCreated { get; set; }

        [Microsoft.Extensions.VectorData.VectorStoreVector(1536)] // Matches text-embedding-3-small dimension size
        public ReadOnlyMemory<float> Vector { get; set; }
    }

    public static class EmailVectorExtensions
    {
        public static EmailSummaryModel ToEmailSummaryModel(this EmailVectorRecord record)
        {
            return new EmailSummaryModel
            {
                Email = record.Email,
                DateCreated = record.DateCreated,
                Id = record.Id,
                Message = record.Message,
                Subject = record.Subject
            };
        }
        public static EmailVectorRecord ToEmailVectorRecord(this EmailSummaryModel emailSummary, ReadOnlyMemory<float>? vector = null)
        {
            return new EmailVectorRecord
            {
                Id = emailSummary.Id,
                Email = emailSummary.Email,
                Subject = emailSummary.Subject,
                Message = emailSummary.Message,
                DateCreated = emailSummary.DateCreated,
                Vector = vector ?? new ReadOnlyMemory<float>()
            };
        }
    }
}


