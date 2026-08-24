using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchurkoPortfolio.Core.Data;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Repository;

namespace SchurkoPortfolio.Pages.Admin
{
    public class EmailChartDetailsModel : PageModel
    {
        public EmailDbContext _dbContext;

        public EmailChartDetailsModel(EmailDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public EmailSummaryModel EmailSummary { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            EmailSummary = await _dbContext.EmailSummary.FirstOrDefaultAsync(cc => cc.Id == id);
            if (EmailSummary == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
