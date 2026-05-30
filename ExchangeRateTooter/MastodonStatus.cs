// ReSharper disable UnusedAutoPropertyAccessor.Global
// All properties must have public getters and setters in order for serialisation to work
namespace ExchangeRateTooter;

public class MastodonStatus
{
    public string Status { get; set; } = string.Empty;
    public string Visibility { get; set; } = "public";
}