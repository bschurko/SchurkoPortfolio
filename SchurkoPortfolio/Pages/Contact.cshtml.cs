using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchurkoPortfolio.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DeveloperPortfolio.Pages;

public class ContactModel : PageModel
{
    [BindProperty]
    public ContactInput Input { get; set; } = new();

    public bool Sent { get; private set; }

    public ISmtpEmailService EmailService { get; set; } = default!;

    public ContactModel(ISmtpEmailService emailService)
    {
        EmailService = emailService;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var success = await EmailService.SendEmailAsync(Input.Email, Input.Subject, Input.Message);

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

