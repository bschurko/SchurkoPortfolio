using CommunityToolkit.VectorData.InMemory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Repository;
using SchurkoPortfolio.Core.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SchurkoPortfolio.Pages.Admin
{

    public class AiEmailChartModel : PageModel
    {
        private readonly IEmailRepository _repository;
        private readonly Kernel _kernel;
        private readonly IEmailAiService aiService;

        public AiEmailChartModel(IEmailRepository repository,
            Kernel kernel, IEmailAiService aiService)
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
            EmailSummaries = await _repository.GetAllAsync();

            var emails = EmailSummaries.Select(cc => cc.ToEmailVectorRecord()).ToList();

            var vectorStore = _kernel.GetRequiredService<VectorStore>();
            var emailCollection = vectorStore.GetCollection<int, EmailVectorRecord>("emails");
            await emailCollection.EnsureCollectionExistsAsync();
            var embeddingGenerationService = _kernel.GetRequiredService<Microsoft.SemanticKernel.Embeddings.ITextEmbeddingGenerationService>();


            foreach (var email in emails)
            {
                string contentToEmbed = $"Id: {email.Id} - " +
                    $"Email: {email.Email} - " +
                    $"Subject: {email.Subject} - " +
                    $"Message: {email.Message} - " +
                    $"DateCreated: {email.DateCreated}";

                email.Vector = await embeddingGenerationService.GenerateEmbeddingAsync(contentToEmbed);
                await emailCollection.UpsertAsync(email);
            }
            const double minSimilarity = 0.55;
            ReadOnlyMemory<float> queryEmbedding =
               await embeddingGenerationService.GenerateEmbeddingAsync(Prompt ?? string.Empty);
            var res = emailCollection.SearchAsync(queryEmbedding, 10, null);
            EmailSummaries.Clear();
            await foreach (var r in res)
            {
                if (r.Score >= minSimilarity)
                {
                    EmailSummaries.Add(r.Record.ToEmailSummaryModel());
                }
            }

            return Page();
        }

        public async Task OnGetAsync()
        {
            EmailSummaries = await _repository.GetAllAsync();

        }
    }
}
