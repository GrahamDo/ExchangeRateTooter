using Newtonsoft.Json;

namespace ExchangeRateTooter
{
    internal class Settings
    {
        private const string SettingsFileName = "settings.json";

        public string ExchangeRateApiKey { get; set; }
        public string BaseCurrencyCode { get; set; }
        public List<string> CompareCurrencyCodes { get; set; }
        public string MastodonToken { get; set; }
        public string MastodonInstanceUrl { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<DateTime> PublicHolidays { get; set; }

        public Settings()
        {
            ExchangeRateApiKey = string.Empty;
            BaseCurrencyCode = string.Empty;
            CompareCurrencyCodes = new List<string>();
            MastodonToken = string.Empty;
            MastodonInstanceUrl = string.Empty;
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
                case "basecurrencycode":
                    BaseCurrencyCode = value;
                    break;
                case "comparecurrencycodes":
                    var currencyCodes = value.Split(',');
                    var currencyCodesList = new List<string>();
                    foreach (var currencyCode in currencyCodes)
                        currencyCodesList.Add(currencyCode);
                    CompareCurrencyCodes = currencyCodesList;
                    break;
                case "mastodontoken":
                    MastodonToken = value;
                    break;
                case "mastodoninstanceurl":
                    MastodonInstanceUrl = value;
                    break;
                case "starttime":
                    StartTime = TryFormatTimeSpan(value);
                    break;
                case "endtime":
                    EndTime = TryFormatTimeSpan(value);
                    break;
                case "publicholidays":
                    var holidays = value.Split(',');
                    var holidaysList = new List<DateTime>();
                    foreach (var holiday in holidays)
                        holidaysList.Add(TryFormatDateTime(holiday));
                    PublicHolidays = holidaysList;
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
