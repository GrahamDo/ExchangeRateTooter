namespace ZarCurrencyTooter
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                var settings = Settings.Load();
                if (args.Length == 3 && args[0].ToLower() == "--set")
                {
                    settings.SetValueFromArguments(args[1], args[2]);
                    settings.Save();
                    return;
                }

                var now = DateTime.Now;
                if (ShouldRun(now, args, settings))
                {
                    var currencies = new CurrenciesApiClient();
                    var rates = await currencies.GetLatest(settings.ExchangeRateApiKey, settings.BaseCurrencyCode,
                        settings.CompareCurrencyCodes);
                    var template = new TootTemplate();
                    var tootText = template.GetTootText(now, settings.BaseCurrencyCode, rates);

                    if (args.Length == 1 && args[0].ToLower() == "--fake")
                    {
                        Console.WriteLine("Not posting to Mastodon. --fake detected.");
                        Console.WriteLine("The following WOULD be posted:");
                        Console.WriteLine();
                        Console.WriteLine(tootText);
                    }
                    else
                    {
                        var mastodon = new MastodonApiClient();
                        await mastodon.Post(settings.MastodonInstanceUrl, settings.MastodonToken, tootText);
                    }
                }
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool ShouldRun(DateTime now, string[] args, Settings settings)
        {
            if (args.Length == 1 && args[0].ToLower() == "--force")
                return true;

            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                throw new ApplicationException($"Not running on {now.DayOfWeek}");

            if (settings.StartTime == new TimeSpan(0, 0, 0, 0) && settings.EndTime == new TimeSpan(0, 0, 0, 0))
                return true;
            if (now.TimeOfDay < settings.StartTime || now.TimeOfDay > settings.EndTime)
                throw new ApplicationException("Not running outside of operating hours");

            settings.PublicHolidays.ForEach(holiday =>
            {
                if (now.Date == holiday.Date)
                    throw new ApplicationException("Not running on a public holiday");
            });

            return true;
        }
    }
}