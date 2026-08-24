using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using SchurkoPortfolio.Core.Data;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Repository;

namespace SchurkoPortfolio.Core.Repository
{

    public interface IEmailRepository
    {
        Task<List<EmailSummaryModel>> SearchAsync(
            EmailFilter filter,
            CancellationToken cancellationToken = default);
        Task<List<EmailSummaryModel>> GetAllAsync();
        Task<EmailSummaryModel?> GetByIdAsync(int id);
    }
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailDbContext _dbContext;

        public EmailRepository(EmailDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<EmailSummaryModel>> GetAllAsync()
        {
            var emails = _dbContext.EmailSummary
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            return emails;
        }

        public Task<EmailSummaryModel?> GetByIdAsync(int id)
        {
            var email = _dbContext.EmailSummary
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
            return email;
        }

        public async Task<List<EmailSummaryModel>> SearchAsync(EmailFilter filter, CancellationToken cancellationToken = default)
        {
            IQueryable<EmailSummaryModel> query =
                _dbContext.EmailSummary.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                query = query.Where(x => x.Email != null && x.Email.Contains(filter.Email));
            }

            if (filter.ToDate != null || filter.FromDate != null)
            {
                query = query.Where(x => x.DateCreated != null && x.DateCreated >= filter.FromDate);
                query = query.Where(x => x.DateCreated != null && x.DateCreated <= filter.ToDate);

            }

            if (!string.IsNullOrWhiteSpace(filter.Keywords))
            {
                query = query.Where(x =>
                    x.Email != null && x.Email.Contains(filter.Keywords) ||
                    x.Subject != null && x.Subject.Contains(filter.Keywords) ||
                    x.Message != null && x.Message.Contains(filter.Keywords));
            }

            return await query
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync(cancellationToken);
        }
    }
}
