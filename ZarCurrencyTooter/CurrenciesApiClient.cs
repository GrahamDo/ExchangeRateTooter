using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace ZarCurrencyTooter
{
    public class CurrenciesApiClient
    {
        public async Task<CurrencyLatestResults> GetLatest(string apiKey, string baseCurrencyCode,
            List<string> compareCurrencyCodes)
        {
            var restClient = new RestClient("https://api.apilayer.com/");
            var symbols = ConvertCurrencyCodesToCsv(compareCurrencyCodes);
            var request = new RestRequest($"exchangerates_data/latest?base={baseCurrencyCode}&symbols={symbols}");
            request.AddHeader("apikey", apiKey);
            var response = await restClient.GetAsync(request);
            var results = JsonConvert.DeserializeObject<CurrencyLatestResults>(response.Content);
            return results;
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
