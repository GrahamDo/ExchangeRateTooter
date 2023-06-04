using System.Text;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace ExchangeRateTooter;

public class MastodonApiClient
{
    private RestClient _restClient = null!;
    private readonly MaxCharactersCacheManager _cacheManager;

    public MastodonApiClient(MaxCharactersCacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    private void InitialiseClient(string instanceUrl, string token)
    {
        if (string.IsNullOrEmpty(instanceUrl))
            throw new ApplicationException("Missing Mastodon instance URL");
        if (string.IsNullOrEmpty(token))
            throw new ApplicationException("Missing Mastodon token");

        var baseUrl = BuildBaseUrl(instanceUrl);
        var options = new RestClientOptions(baseUrl)
        {
            Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer")
        };
        _restClient = new RestClient(options);
    }

    private static string BuildBaseUrl(string instanceUrl)
    {
        var baseUrlSb = new StringBuilder();
        if (!instanceUrl.StartsWith("https://"))
            baseUrlSb.Append("https://");
        baseUrlSb.Append(instanceUrl);
        if (!instanceUrl.EndsWith("/"))
            baseUrlSb.Append("/");
        baseUrlSb.Append("api/v1/");
        return baseUrlSb.ToString();
    }

    private async Task<int> GetTootCharacterLimit()
    {
        var charLimit = _cacheManager.GetMaxCharacters();
        if (charLimit > 0)
            return charLimit;

        // This assumes the client has been initialised by the Post method
        var request = new RestRequest("instance");
        var response = await _restClient.GetAsync(request);
        if (response?.Content == null)
            throw new ApplicationException("Empty response from getting instance info");

        var results = JsonConvert.DeserializeObject<InstanceInfo>(response.Content);
        if (results == null)
            throw new ApplicationException("Can't deserialise instance info");

        charLimit = results.Configuration.Statuses.MaxChars;
        _cacheManager.SaveMaxCharacters(charLimit);
        return charLimit;
    }

    public async Task Post(string instanceUrl, string token, string text, bool isDirect = false)
    {
        InitialiseClient(instanceUrl, token);
        var charLimit = await GetTootCharacterLimit();
        var status = new MastodonStatus
        {
            Status = ShortenText(text, charLimit)
        };
        if (isDirect)
            status.Visibility = "direct";

        var request = new RestRequest("statuses", Method.Post).AddJsonBody(status);
        try
        {
            await _restClient.PostAsync(request);
        }
        catch (HttpRequestException ex)
        {
            if (ex.Message.Contains("Forbidden"))
                throw new ApplicationException("Invalid Mastodon token");

            throw;
        }
    }

    private string ShortenText(string text, int charLimit)
        => text.Length <= charLimit ? text : text.Substring(0, charLimit);
}