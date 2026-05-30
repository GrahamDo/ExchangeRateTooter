// ReSharper disable CollectionNeverUpdated.Global
// All properties must have public getters and setters in order for serialisation to work

namespace ExchangeRateTooter;

public class CurrencyLatestResults
{
    public Dictionary<string, float> Rates { get; set; } = new();
}