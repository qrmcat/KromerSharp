using System.Text.Json.Serialization;
using Kromer.Utils;

namespace Kromer.Models.Api.Krist.Lookup;

public class KristExtendedSearchResult : KristResult
{
    public ParsedQuery Query { get; set; }
    
    public MatchResult Matches { get; set; }

    public class MatchResult
    {
        public MatchTransactions Transactions { get; set; }
    }

    public class MatchTransactions
    {
        [JsonPropertyName("addressInvolved")]
        [JsonConverter(typeof(NullAsFalseConverterFactory))]
        public int? AddressInvolved { get; set; }
        
        [JsonPropertyName("nameInvolved")]
        [JsonConverter(typeof(NullAsFalseConverterFactory))]
        public int? NameInvolved { get; set; }
        
        [JsonPropertyName("metadata")]
        [JsonConverter(typeof(NullAsFalseConverterFactory))]
        public int? Metadata { get; set; }
    }
}