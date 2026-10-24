using System.Text;
using System.Text.Json;

namespace ExchangeRateTooter
{
    public class CurrenciesApiClient(byte mainMaxRetries, int mainRetryWaitMilliseconds, HttpClientFactory clientFactory)
    {
        private readonly HttpClient _client = clientFactory.GetClient();

        public async Task<CurrencyLatestResults> GetLatest(string apiKey, string baseCurrencyCode,
            List<string> compareCurrencyCodes, byte retryCount = 0)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ApplicationException("Exchange Rate API Key not specified");

            var symbols = ConvertCurrencyCodesToCsv(compareCurrencyCodes);
            var requestUrl = $"https://api.apilayer.com/exchangerates_data/latest?base={baseCurrencyCode}&symbols={symbols}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("apikey", apiKey);

            using var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                    case System.Net.HttpStatusCode.Forbidden:
                        throw new ApplicationException("Invalid Exchange Rate API Key");
                    case System.Net.HttpStatusCode.TooManyRequests:
                        throw new ApplicationException("Too many requests to Exchange Rate API");
                    case System.Net.HttpStatusCode.RequestTimeout:
                    case System.Net.HttpStatusCode.GatewayTimeout:
                        if (retryCount == mainMaxRetries)
                            throw new ApplicationException(
                                $"Exchange Rate API returned status code {response.StatusCode}. Retry limit exceeded.");

                        retryCount++;
                        Thread.Sleep(mainRetryWaitMilliseconds);
                        return await GetLatest(apiKey, baseCurrencyCode, compareCurrencyCodes, retryCount);

                    default:
                        throw new ApplicationException(
                            $"Exchange Rate API Client returned status code {response.StatusCode}");
                }
            }

            if (response.Content == null)
                throw new ApplicationException("Empty response from Exchange Rate API Client");

            var json = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<CurrencyLatestResults>(json);
            if (results == null)
                throw new ApplicationException("Can't deserialise Exchange Rate Content");

            return results;
        }

        private static string ConvertCurrencyCodesToCsv(List<string> currencyCodes)
        {
            var result = new StringBuilder();
            currencyCodes.ForEach(currencyCode =>
            {
                if (result.Length > 0)
                    result.Append(",");
                result.Append(currencyCode);
            });
            return result.ToString();
        }
    }
}
