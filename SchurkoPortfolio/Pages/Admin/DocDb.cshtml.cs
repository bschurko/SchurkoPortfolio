using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Services;
using SchurkoPortfolio.Core.Utils;

namespace SchurkoPortfolio.Pages.Admin
{
    public class DocDbModel : PageModel
    {
        private IImageUploadService _uploadService { get; set; }
        public DocDbModel(IImageUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [BindProperty]
        public ImageUploadRequest ImageUploadRequest { get; set; }
        [BindProperty]
        public string SuccessMessage { get; set; } = string.Empty;
        [BindProperty]
        public IEnumerable<ImageDocument> ImageDocuments { get; set; } = Enumerable.Empty<ImageDocument>();
        [BindProperty]
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                ImageDocuments = await _uploadService.GetAllAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error retrieving images: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken ct)
        {
            if (this.ImageUploadRequest.File is null)
            {
                ErrorMessage = "Please choose a file.";
                return RedirectToPage();
            }

            try
            {
                var doc = await _uploadService.UploadAsync(ImageUploadRequest, ct);

                if (doc != null)
                {
                    SuccessMessage = $"Uploaded '{doc.FileName}' successfully.";
                }
                else
                {
                    ErrorMessage = "Failed to upload the image. Please try again.";
                }
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }

            return RedirectToPage(); // PRG pattern — avoids resubmission on refresh
        }
    }
}
