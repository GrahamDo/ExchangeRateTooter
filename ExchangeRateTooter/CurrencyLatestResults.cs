namespace ExchangeRateTooter;

public class CurrencyLatestResults
{
    public Dictionary<string, float> Rates { get; set; }

    public CurrencyLatestResults()
    {
        Rates = new Dictionary<string, float>();
    }
}