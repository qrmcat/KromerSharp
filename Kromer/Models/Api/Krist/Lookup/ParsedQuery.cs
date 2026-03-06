using System.Text.Json.Serialization;

namespace Kromer.Models.Api.Krist.Lookup;

public class ParsedQuery
{
    [JsonPropertyName("originalQuery")]
    public string OriginalQuery { get; set; }
        
    [JsonPropertyName("matchAddress")]
    public bool MatchAddress { get; set; }

    [JsonPropertyName("matchName")]
    public bool MatchName { get; set; }

    [JsonPropertyName("matchBlock")]
    public bool MatchBlock { get; set; } = false;

    [JsonPropertyName("matchTransaction")]
    public bool MatchTransaction { get; set; }

    [JsonPropertyName("strippedName")]
    public string StrippedName { get; set; }

    [JsonPropertyName("hasID")] public bool HasId { get; set; }

    [JsonPropertyName("cleanID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CleanId { get; set; }
}