using System.Text;

namespace ZarCurrencyTooter;

public class TootTemplate
{
    private const string TemplateFileName = "toot-template.txt";

    public string GetTootText(DateTime currentDateTime, string baseCurrencyCode, CurrencyLatestResults rates)
    {
        var templateText = Load();
        var ratesText = GetRatesText(rates);
        return templateText.Replace("{DateTime}", currentDateTime.ToString("yyyy-MM-dd HH:mm"))
            .Replace("{BaseCurrency}", baseCurrencyCode)
            .Replace("{Rates}", ratesText);

    }

    private static string GetRatesText(CurrencyLatestResults rates)
    {
        var result = new StringBuilder();
        foreach (var keyValuePair in rates.Rates)
        {
            var value = 1 / keyValuePair.Value;
            result.AppendLine($"{keyValuePair.Key}: {value:#0.0000}");
        }
        return result.ToString();
    }

    private static string Load()
    {
        if (!File.Exists(TemplateFileName))
            throw new ApplicationException($"{TemplateFileName} not found");

        var text = File.ReadAllText(TemplateFileName);
        return text;
    }
}