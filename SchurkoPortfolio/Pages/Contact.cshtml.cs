using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeveloperPortfolio.Pages;

public class ContactModel : PageModel
{
    [BindProperty]
    public ContactInput Input { get; set; } = new();

    public bool Sent { get; private set; }

    public void OnGet() { }

    public void OnPost()
    {
        if (!ModelState.IsValid)
            return;

        // Replace this with your email provider, database, or CRM integration.
        Sent = true;
        ModelState.Clear();
        Input = new();
    }

    public class ContactInput
    {
        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string Subject { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Message { get; set; } = string.Empty;
    }
}
