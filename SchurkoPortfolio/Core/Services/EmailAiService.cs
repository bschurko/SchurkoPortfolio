using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SchurkoPortfolio.Core.Model;
using System.Text.Json;
using System.Threading;

namespace SchurkoPortfolio.Core.Services
{
    public interface IEmailAiService
    {
        public Task<EmailFilter> CreateEmailFilterAsync(string prompt, CancellationToken cancellationToken = default);
    }

    public class EmailAiService : IEmailAiService
    {
        public Kernel _kernel { get; set; }

        public EmailAiService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<EmailFilter> CreateEmailFilterAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var settings = new OpenAIPromptExecutionSettings
            {
                ResponseFormat = typeof(EmailFilter)
            };

            var instructions = """
            You convert natural language email summary searches into a structured
            EmailFilter object.

            Rules:

            - Do not invent values.
            - If a filter is not specified, return null.
            - DateCreated represents the date time the email was created.
            - Subject represents the title of the email
            - Message represents the containing message of the email
            - Dates must be ISO 8601.
            - if filter FromDate is supplied its used as FromDate > DateCreated
            - if filter ToDate is supplied its used as ToDate < DateCreated
            - Put concepts that should be semantically searched into Keywords.
            """;

            var args = new KernelArguments
            {
                ["prompt"] = prompt,
                ExecutionSettings = new Dictionary<string, PromptExecutionSettings>
                {
                    [PromptExecutionSettings.DefaultServiceId] = settings
                }
            };

            var template = instructions + "\n\nUser request:\n{{$prompt}}\n";

            var result = await _kernel.InvokePromptAsync(
                template,
                args,
                cancellationToken: cancellationToken);

            var json = result.ToString();

            var filter = JsonSerializer.Deserialize<EmailFilter>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (filter == null)
            {
                throw new InvalidOperationException(
                    "AI returned an invalid email filter.");
            }

            return filter;
        }
    }
}
