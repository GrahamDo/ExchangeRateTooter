namespace ExchangeRateTooter;

public class MastodonStatus
{
    public string Status { get; set; }
    public string Visibility { get; set; }

    public MastodonStatus()
    {
        Status = string.Empty;
        Visibility = "public";
    }
}