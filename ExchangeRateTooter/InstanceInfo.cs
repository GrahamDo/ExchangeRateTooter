using Newtonsoft.Json;

namespace ExchangeRateTooter
{
    public class InstanceInfo
    {
        public InstanceInfoConfiguration Configuration { get; set; } = new();
    }

    public class InstanceInfoConfiguration
    {
        public InstanceInfoConfigurationStatuses Statuses { get; set; } = new();
    }

    public class InstanceInfoConfigurationStatuses
    {
        [JsonProperty("max_characters")]
        public int MaxChars { get; set; }
    }
}
