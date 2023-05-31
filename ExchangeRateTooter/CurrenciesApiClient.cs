using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace ExchangeRateTooter
{
    public class CurrenciesApiClient
    {
        private readonly byte _maxRetries;
        private readonly int _retryWaitMilliseconds;

        public CurrenciesApiClient(byte maxRetries, int retryWaitMilliseconds)
        {
            _maxRetries = maxRetries;
            _retryWaitMilliseconds = retryWaitMilliseconds;
        }

        public async Task<CurrencyLatestResults> GetLatest(string apiKey, string baseCurrencyCode,
            List<string> compareCurrencyCodes, byte retryCount = 0)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ApplicationException("Exchange Rate API Key not specified");

            var restClient = new RestClient("https://api.apilayer.com/");
            var symbols = ConvertCurrencyCodesToCsv(compareCurrencyCodes);
            var request = new RestRequest($"exchangerates_data/latest?base={baseCurrencyCode}&symbols={symbols}");
            request.AddHeader("apikey", apiKey);
            try
            {
                var response = await restClient.GetAsync(request);
                if (response?.Content == null)
                    throw new ApplicationException("Empty response from Exchange Rate API Client");

                var results = JsonConvert.DeserializeObject<CurrencyLatestResults>(response.Content);
                if (results == null)
                    throw new ApplicationException("Can't deserialise Exchange Rate Content");

                return results;
            }
            catch (TimeoutException)
            {
                if (retryCount < _maxRetries)
                {
                    retryCount++;
                    Thread.Sleep(_retryWaitMilliseconds);
                    return await GetLatest(apiKey, baseCurrencyCode, compareCurrencyCodes, retryCount);
                } 

                throw;
            }
            catch (HttpRequestException e)
            {
                if (e.Message.Contains("Unauthorized"))
                    throw new ApplicationException("Invalid Exchange Rate API Key");
                if (e.StatusCode != null && (uint)e.StatusCode == 522 && retryCount < _maxRetries)
                {
                    // 522 is the Cloudflare-specific error meaning Timeout
                    retryCount++;
                    Thread.Sleep(_retryWaitMilliseconds);
                    return await GetLatest(apiKey, baseCurrencyCode, compareCurrencyCodes, retryCount);
                }

                throw;
            }
        }

        private string ConvertCurrencyCodesToCsv(List<string> currencyCodes)
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
