using System.Net;
using System.Text;
using System.Text.Json;


namespace ExchangeRateTooter;

public class MastodonApiClient(MaxCharactersCacheManager mainCacheManager, HttpClientFactory clientFactory)
{
    private readonly HttpClient _client = clientFactory.GetClient();

    private static string BuildBaseUrl(string instanceUrl)
    {
        var baseUrlSb = new StringBuilder();
        if (!instanceUrl.StartsWith("https://"))
            baseUrlSb.Append("https://");
        baseUrlSb.Append(instanceUrl);
        if (!instanceUrl.EndsWith('/'))
            baseUrlSb.Append('/');
        baseUrlSb.Append("api/v1/");
        return baseUrlSb.ToString();
    }

    private async Task<int> GetTootCharacterLimit(string baseUrl, string token)
    {
        var charLimit = mainCacheManager.GetMaxCharacters();
        if (charLimit > 0)
            return charLimit;

        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}instance")
        {
            Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token) }
        };

        using var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
            throw new ApplicationException("Empty response from getting instance info");

        var results = JsonSerializer.Deserialize<InstanceInfo>(content) ??
                      throw new ApplicationException("Can't deserialise instance info");

        charLimit = results.Configuration.Statuses.MaxChars;
        mainCacheManager.SaveMaxCharacters(charLimit);
        return charLimit;
    }

    public async Task Post(string instanceUrl, string token, string text, bool isDirect = false, bool isRetry = false)
    {
        var baseUrl = BuildBaseUrl(instanceUrl);
        var charLimit = await GetTootCharacterLimit(baseUrl, token);
        var status = new MastodonStatus
        {
            Status = ShortenText(text, charLimit)
        };
        if (isDirect)
            status.Visibility = "direct";

        var json = JsonSerializer.Serialize(status);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}statuses")
        {
            Content = content,
            Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token) }
        };        

        using var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApplicationException("Invalid Mastodon token");
                
            if (response.StatusCode == HttpStatusCode.UnprocessableEntity && !isRetry)
            {
                // It could be the character limit that's decreased. Clear the cache and try again once
                mainCacheManager.ClearCache();
                await Post(instanceUrl, token, text, isDirect, isRetry: true);
                return;
            }

            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }        
    }

    private static string ShortenText(string text, int charLimit)
        => text.Length <= charLimit ? text : text.Substring(0, charLimit);
}