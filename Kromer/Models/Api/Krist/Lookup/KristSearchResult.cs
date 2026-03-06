using System.Text.Json.Serialization;
using Kromer.Models.Dto;
using Kromer.Utils;

namespace Kromer.Models.Api.Krist.Lookup;

public class KristSearchResult : KristResult
{
    public ParsedQuery Query { get; set; }
    
    public MatchResult Matches { get; set; }

    public class MatchResult
    {
        [JsonPropertyName("exactAddress")]
        [JsonConverter(typeof(NullAsFalseConverterFactory))]
        public AddressDto? ExactAddress { get; set; }

        [JsonPropertyName("exactName")]
        [JsonConverter(typeof(NullAsFalseConverterFactory))]
        public NameDto? ExactName { get; set; }

        [JsonPropertyName("exactBlock")]
        public bool ExactBlock => false;

        [JsonPropertyName("exactTransaction")]
        [JsonConverter(typeof(NullAsFalseConverterFactory))]
        public TransactionDto? ExactTransaction { get; set; }
    }
}