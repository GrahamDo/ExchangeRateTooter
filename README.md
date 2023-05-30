# ExchangeRateTooter

This is a console app designed to be run as a bot. It posts a preconfigured list of exchange rates to Mastodon. For an example implementation, see https://mastodon.africa/@ZARExchangeRates

## Prerequisite

In order to use this bot, you'll need an account at https://apilayer.com/ with a subscription to the ExchangeRatesData API. Don't worry, it's free for up to 250 requests per month.

You'll also need a Mastodon Access Token with permission to <code>write:statuses</code>. You can get one of those by going to Preferences -> Development in your Mastodon account.

## Getting it running

Pull the code and build it for your preferred platform. I've tested it on both Windows x64 and Ubuntu 23.04 x64.

Then open a terminal and run the following for each setting to configure everything:

<code>ExchangeRateTooter --set &lt;setting&gt; &lt;value&gt;</code>

### Settings

* ExchangeRateApiKey - The API Key you received from apilayer.com
* BaseCurrencyCode - The currency code to get exchange rates for (e.g. <code>USD</code>)
* CompareCurrencyCodes - A comma-separated list of currencies to compare with the base (e.g. <code>EUR,GBP,AUD</code>)
* CurrenciesMaxRetryCount - Should the currencies API time out, how many times should we retry? The default is <code>0</code> (i.e. don't retry)
* CurrenciesRetryWaitMilliseconds - How many milliseconds should we wait between retries? The default is <code>60000</code> (i.e. one minute)
* MastodonToken - Your Mastodon Access Token
* MastodonInstanceUrl - The URL of your Mastodon instance (e.g. <code>mastodon.africa</code>)
* StartTime - Don't run before this time of day (e.g. <code>09:00</code>)
* EndTime - Don't run after this time of day (e.g. <code>17:00</code>)
* PublicHolidays - A comma separated list of dates on which you don't want the app to run (it automatically won't run on Saturdays or Sundays) (e.g. <code>2023-01-01,2023-03-21,2023-12-25</code>)

### Running

Once everything is set up, run:

<code>ExchangeRateTooter --fake</code>

This will connect to the Exchange Rates API and print what it would toot to the console. If you're not happy with that, edit the <code>toot-template.txt</code>, which should be self-explanatory.

Once you're happy, run <code>ExchangeRateTooter</code> without any arguments, and it will do everything silently and post to Mastodon.

Then create a cron job or Windows Scheduled Task to do that as often as you like, and you're good to go! :-)
