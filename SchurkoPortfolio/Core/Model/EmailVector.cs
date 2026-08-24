using Microsoft.Extensions.VectorData;

namespace SchurkoPortfolio.Core.Model
{
    public class EmailVector
    {
        [VectorStoreKey]
        public string Key { get; set; } = string.Empty; // string form of your email Id, used as Cosmos "id" and partition key

        [VectorStoreData(IsIndexed = true)]
        public int Id { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public string Email { get; set; } = string.Empty;

        [VectorStoreData]
        public string Subject { get; set; } = string.Empty;

        [VectorStoreData]
        public string Message { get; set; } = string.Empty;

        [VectorStoreData(IsIndexed = true)]
        public DateTimeOffset DateCreated { get; set; }

        [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
}
