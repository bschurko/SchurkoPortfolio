using Microsoft.Azure.Cosmos;
using MongoDB.Driver;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Utils;
using System.ComponentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SchurkoPortfolio.Core.Services
{
    public interface IImageUploadService
    {
        Task<IEnumerable<ImageDocument>> GetAllAsync(CancellationToken ct = default);
        Task<ImageDocument> UploadAsync(ImageUploadRequest request, CancellationToken ct = default);
        Task<ImageDocument?> GetAsync(string id, CancellationToken ct = default);
        Task<byte[]> GetImageBytesAsync(string id, CancellationToken ct = default);
    }
    public class ImageUploadService : IImageUploadService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly string[] AllowedContentTypes =
            { "image/png", "image/jpeg", "image/webp", "image/gif" };

        // Raw (pre-base64) size limit — deliberately well under the 2 MB Cosmos
        // document cap, since base64 adds ~33% and you still need room for the
        // other fields on the document.
        private const long MaxRawFileSizeBytes = 500 * 1024; // 500 KB
        private readonly MongoDbSettings _mongoSettings;
        private IMongoDatabase _mongoDatabase { get; set; }

        public ImageUploadService(IMongoClient mongoClient,
            MongoDbSettings mongoSettings,
            IHttpContextAccessor httpContextAccessor)
        {

            _mongoSettings = mongoSettings;
            _httpContextAccessor = httpContextAccessor;
            _mongoDatabase = mongoClient.GetDatabase(_mongoSettings.DatabaseName);
        }

        public async Task<ImageDocument> UploadAsync(ImageUploadRequest request, CancellationToken ct = default)
        {
            if (request.File is null || request.File.Length == 0)
                throw new ArgumentException("No file was uploaded.");

            if (request.File.Length > MaxRawFileSizeBytes)
            {
                throw new ArgumentException(
                    $"File is {request.File.Length / 1024} KB — exceeds the {MaxRawFileSizeBytes / 1024} KB " +
                    "limit for inline storage in Cosmos DB. Use Blob Storage for larger images.");
            }

            if (!AllowedContentTypes.Contains(request.File.ContentType))
                throw new ArgumentException($"Unsupported content type: {request.File.ContentType}");

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.File.CopyToAsync(memoryStream, ct);
                imageBytes = memoryStream.ToArray();
            }

            var doc = new ImageDocument
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                SizeBytes = imageBytes.Length,
                Base64Data = Convert.ToBase64String(imageBytes),
                Title = request.Title,
                Description = request.Description,
                Tags = string.IsNullOrWhiteSpace(request.Tags)
                    ? new List<string>()
                    : request.Tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()
            };

            // Sanity-check the actual serialized document size before writing —
            // catches edge cases where metadata fields (long description, many tags)
            // push a borderline-sized image over the 2 MB cap.
            var estimatedSize = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(doc).Length;
            const int cosmosDocumentLimitBytes = 2 * 1024 * 1024;
            if (estimatedSize >= cosmosDocumentLimitBytes)
            {
                throw new InvalidOperationException(
                    $"Document size ({estimatedSize / 1024} KB) would exceed Cosmos DB's 2 MB document limit.");
            }

            var collection = _mongoDatabase.GetCollection<ImageDocument>("images");
            await collection.InsertOneAsync(doc, cancellationToken: ct);

            return doc;
        }

        public async Task<ImageDocument?> GetAsync(string id, CancellationToken ct = default)
        {
            try
            {
                var collection = _mongoDatabase.GetCollection<ImageDocument>("images");

                var filter = Builders<ImageDocument>.Filter.Eq(d => d.Id, id);
                var ret = collection.FindAsync<ImageDocument>(filter, cancellationToken: ct);

                return await ret.Result.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<byte[]> GetImageBytesAsync(string id, CancellationToken ct = default)
        {
            var doc = await GetAsync(id, ct)
                ?? throw new KeyNotFoundException($"Image {id} not found.");

            return Convert.FromBase64String(doc.Base64Data);
        }

        public async Task<IEnumerable<ImageDocument>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var collection = _mongoDatabase.GetCollection<ImageDocument>("images");
                return await collection.Find(Builders<ImageDocument>.Filter.Empty).ToListAsync(ct);
            }
            catch (Exception ex)
            {
                Log.Logger.LogError("Error retrieving all images: {Message}", ex.Message);
                throw;
            }
        }
    }
}
