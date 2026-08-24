namespace SchurkoPortfolio.Core.Model
{
    public class EmailFilter
    {
        public string Email { get; set; }
        public string Keywords { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
