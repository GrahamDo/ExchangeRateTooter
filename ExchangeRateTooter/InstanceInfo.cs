using System.Text.Json.Serialization;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// All properties must be read/write for serialization

namespace ExchangeRateTooter;

internal class InstanceInfo
{
    [JsonPropertyName("configuration")]
    public InstanceInfoConfiguration Configuration { get; init; } = new();
}

internal class InstanceInfoConfiguration
{
    [JsonPropertyName("statuses")]
    public InstanceInfoConfigurationStatuses Statuses { get; init; } = new();
}

internal class InstanceInfoConfigurationStatuses
{
    [JsonPropertyName("max_characters")]
    public int MaxChars { get; init; }
}