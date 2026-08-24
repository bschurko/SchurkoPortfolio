using MongoDB.Bson.Serialization.Attributes;

namespace SchurkoPortfolio.Core.Model
{
    public class ImageDocument
    {

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("FileName")]
        public string FileName { get; set; } = string.Empty;

        [BsonElement("ContentType")]
        public string ContentType { get; set; } = string.Empty;

        [BsonElement("SizeBytes")]
        public long SizeBytes { get; set; }

        [BsonElement("Base64Data")]
        public string Base64Data { get; set; } = string.Empty;       // the image itself, inline

        [BsonElement("CreatedDate")]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        // ---- your "other information" fields go here ----

        [BsonElement("Title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("Tags")]
        public List<string> Tags { get; set; }
    }
}
