using Newtonsoft.Json;
using System.Xml.Linq;

namespace ZarCurrencyTooter
{
    internal class Settings
    {
        private const string SettingsFileName = "settings.json";

        public string ExchangeRateApiKey { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<DateTime> PublicHolidays { get; set; }

        public Settings()
        {
            PublicHolidays = new List<DateTime>();
        }

        public static Settings Load()
        {
            if (!File.Exists(SettingsFileName))
                return new Settings();

            var text = File.ReadAllText(SettingsFileName);
            return JsonConvert.DeserializeObject<Settings>(text) ??
                   throw new ApplicationException($"Your '{SettingsFileName}' appears to be empty or corrupt.");
        }

        public void Save()
        {
            var serialised = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(SettingsFileName, serialised);
        }

        public void SetValueFromArguments(string setting, string value)
        {
            switch (setting.ToLower())
            {
                case "exchangerateapikey":
                    ExchangeRateApiKey = value;
                    break;
                case "starttime":
                    StartTime = TryFormatTimeSpan(value);
                    break;
                case "endtime":
                    EndTime = TryFormatTimeSpan(value);
                    break;
                case "publicholidays":
                    var elements = value.Split(',');
                    var result = new List<DateTime>();
                    foreach (var element in elements)
                        result.Add(TryFormatDateTime(element));
                    PublicHolidays = result;
                    break;
                default:
                    throw new ApplicationException($"Invalid setting: {setting}");
            }
        }

        public TimeSpan TryFormatTimeSpan(string value)
        {
            var isTimeSpan = TimeSpan.TryParse(value, out var result);
            if (!isTimeSpan)
                throw new ApplicationException($"{value} is not a valid time");

            return result;
        }
        public DateTime TryFormatDateTime(string value)
        {
            var isDateTime = DateTime.TryParse(value, out var result);
            if (!isDateTime)
                throw new ApplicationException($"{value} is not a valid date");

            return result;
        }
    }
}
