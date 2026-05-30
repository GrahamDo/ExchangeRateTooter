using Newtonsoft.Json;
// ReSharper disable MemberCanBePrivate.Global
// All properties must have public getters and setters in order for serialisation to work

namespace ExchangeRateTooter
{
    internal class Settings
    {
        private const string SettingsFileName = "settings.json";

        public string ExchangeRateApiKey { get; set; } = string.Empty;
        public string BaseCurrencyCode { get; set; } = string.Empty;
        public List<string> CompareCurrencyCodes { get; set; } = [];
        public byte CurrenciesMaxRetryCount { get; set; }
        public int CurrenciesRetryWaitMilliseconds { get; set; } = 60000;
        public string MastodonToken { get; set; } = string.Empty;
        public string MastodonInstanceUrl { get; set; } = string.Empty;
        public string DmAccountName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<DateTime> PublicHolidays { get; set; } = [];

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
                case "currenciesmaxretrycount":
                    CurrenciesMaxRetryCount = TryConvertByte(value);
                    break;
                case "currenciesretrywaitmilliseconds":
                    CurrenciesRetryWaitMilliseconds = TryConvertInt(value, true);
                    break;
                case "mastodontoken":
                    MastodonToken = value;
                    break;
                case "mastodoninstanceurl":
                    MastodonInstanceUrl = value;
                    break;
                case "dmaccountname":
                    DmAccountName = value;
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

        private static byte TryConvertByte(string value)
        {
            var isByte = byte.TryParse(value, out var result);
            if (!isByte)
                throw new ApplicationException("Value should be a whole number between 0 and 255");

            return result;
        }

        private static int TryConvertInt(string value, bool mustBePositive)
        {
            var isInt = int.TryParse(value, out var result);
            if (!isInt || (mustBePositive && result < 0))
                throw new ApplicationException("Value should be a valid whole number");

            return result;
        }

        private static TimeSpan TryFormatTimeSpan(string value)
        {
            var isTimeSpan = TimeSpan.TryParse(value, out var result);
            if (!isTimeSpan)
                throw new ApplicationException($"{value} is not a valid time");

            return result;
        }
        private static DateTime TryFormatDateTime(string value)
        {
            var isDateTime = DateTime.TryParse(value, out var result);
            if (!isDateTime)
                throw new ApplicationException($"{value} is not a valid date");

            return result;
        }
    }
}