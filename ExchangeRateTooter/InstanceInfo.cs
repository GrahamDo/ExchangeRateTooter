using Newtonsoft.Json;

namespace ExchangeRateTooter
{

    public class InstanceInfo
    {
        public InstanceInfo()
        {
            Configuration = new InstanceInfoConfiguration();
        }

        public InstanceInfoConfiguration Configuration { get; set; }
    }

    public class InstanceInfoConfiguration
    {
        public InstanceInfoConfiguration()
        {
            Statuses = new InstanceInfoConfigurationStatuses();
        }

        public InstanceInfoConfigurationStatuses Statuses { get; set; }
    }

    public class InstanceInfoConfigurationStatuses
    {
        [JsonProperty("max_characters")]
        public int MaxChars { get; set; }
    }
}
