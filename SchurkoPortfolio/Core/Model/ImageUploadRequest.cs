using Microsoft.AspNetCore.Mvc;

namespace SchurkoPortfolio.Core.Model
{
    public class ImageUploadRequest
    {
        [FromForm]
        public IFormFile File { get; set; } = default!;

        [FromForm]
        public string Title { get; set; } = string.Empty;

        [FromForm]
        public string Description { get; set; } = string.Empty;

        // Comma-separated tags, e.g. "sunset,beach,vacation"
        [FromForm]
        public string? Tags { get; set; }
    }
}
