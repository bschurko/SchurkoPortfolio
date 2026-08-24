using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchurkoPortfolio.Core.Data;
using SchurkoPortfolio.Core.Interfaces;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Repository;
using SchurkoPortfolio.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace DeveloperPortfolio.Pages;

public class ContactModel : PageModel
{
    [BindProperty]
    public ContactInput Input { get; set; } = new();

    public bool Sent { get; private set; }

    public ISmtpEmailService EmailService { get; set; } = default!;
    public EmailDbContext DbContext { get; }

    public ContactModel(ISmtpEmailService emailService, EmailDbContext dbContext)
    {
        EmailService = emailService;
        DbContext = dbContext;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var success = await EmailService.SendEmailAsync(Input.Email, Input.Subject, Input.Message);
        if (!success)
        {
            Log.Logger.LogError($"Failed to send email message from email {Input.Email} with subject {Input.Subject}");
        }

        EmailSummaryModel model = new()
        {
            Email = Input.Email,
            Subject = Input.Subject,
            Message = Input.Message,
            DateCreated = DateTime.Now
        };
        await DbContext.EmailSummary.AddAsync(model);
        var ret = await DbContext.SaveChangesAsync();
        if (ret == 0)
        {
            Log.Logger.LogError($"Failed to save email summary to database for email {model.Email}");
        }

        Sent = success;
        ModelState.Clear();
        Input = new();

        return Page();
    }

    public class ContactInput
    {

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string Subject { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Message { get; set; } = string.Empty;
    }
}

