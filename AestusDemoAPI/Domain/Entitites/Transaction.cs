using System.Text.Json.Serialization;

namespace AestusDemoAPI.Domain.Entitites
{
    public class Transaction
    {
        public int Id { get; set; }

        [JsonPropertyName("user_id")]
        public required string UserId { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("location")]
        public required string Location { get; set; }

        [JsonPropertyName("isSuspicious")]
        public bool IsSuspicious { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; } = string.Empty;
    }
}
