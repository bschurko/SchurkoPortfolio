using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.SemanticKernel;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Repository;
using SchurkoPortfolio.Core.Services;

namespace SchurkoPortfolio.Pages.Admin
{
    public class EmailChartModel : PageModel
    {
        private readonly IEmailRepository _repository;
        private readonly Kernel _kernel;
        private readonly IEmailAiService aiService;

        public EmailChartModel(IEmailRepository repository, Kernel kernel, IEmailAiService aiService)
        {
            _repository = repository;
            _kernel = kernel;
            this.aiService = aiService;
        }

        [BindProperty]
        public List<EmailSummaryModel> EmailSummaries { get; private set; } = [];

        [BindProperty]
        public string Prompt { get; set; }


        public string GeneratedMessage { get; set; }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(Prompt))
            {
                EmailSummaries = await _repository.GetAllAsync();
                return Page();
            }

            var filter = await this.aiService.CreateEmailFilterAsync(
                Prompt,
                cancellationToken);

            EmailSummaries = await _repository.SearchAsync(
                filter,
                cancellationToken);

            return Page();
        }

        public async Task OnGetAsync()
        {
            EmailSummaries = await _repository.GetAllAsync();

        }
    }
}
