using Microsoft.EntityFrameworkCore;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Repository;

namespace SchurkoPortfolio.Core.Data
{
    public class EmailDbContext : DbContext
    {
        public EmailDbContext(DbContextOptions<EmailDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmailSummaryModel> EmailSummary => Set<EmailSummaryModel>();

    }
}
