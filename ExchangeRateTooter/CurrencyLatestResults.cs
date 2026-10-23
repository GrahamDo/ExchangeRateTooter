// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// All properties must have public getters and setters in order for serialisation to work

using System.Text.Json.Serialization;

namespace ExchangeRateTooter;

public class CurrencyLatestResults
{
    [JsonPropertyName("rates")] public Dictionary<string, float> Rates { get; set; } = new();
}