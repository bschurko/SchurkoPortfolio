namespace SchurkoPortfolio.Core.Model
{
    public class EmailSummaryModel
    {
        public int Id { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Subject { get; set; } = string.Empty;
        public string? Message { get; set; } = string.Empty;
        public DateTime? DateCreated { get; set; }
    }
}
