using System.Text.Json.Serialization;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// All properties must be read/write for serialization
// All properties must be read/write, and all classes concrete for serialization

namespace ExchangeRateTooter;

public class MastodonStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    [JsonPropertyName("visibility")]
    public string Visibility { get; set; } = "public";
}