using Newtonsoft.Json;

namespace ZarCurrencyTooter
{
    internal class Settings
    {
        private const string SettingsFileName = "settings.json";

        public string ExchangeRateApiKey { get; set; }
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
            var serialised = JsonConvert.SerializeObject(this);
            File.WriteAllText(SettingsFileName, serialised);
        }

        public void SetValueFromArguments(string setting, string value)
        {
            switch (setting.ToLower())
            {
                case "exchangerateapikey":
                    ExchangeRateApiKey = value;
                    break;
                case "publicholidays":
                    var elements = value.Split(',');
                    var result = new List<DateTime>();
                    foreach (var element in elements)
                    {
                        try
                        {
                            result.Add(Convert.ToDateTime(element));
                        }
                        catch (FormatException)
                        {
                            throw new ApplicationException($"{element} is not a valid date");
                        }
                    }

                    PublicHolidays = result;
                    break;
                default:
                    throw new ApplicationException($"Invalid setting: {setting}");
            }
        }
    }
}
