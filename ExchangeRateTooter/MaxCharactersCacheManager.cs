using System.Runtime.InteropServices.JavaScript;

namespace ExchangeRateTooter
{
    public class MaxCharactersCacheManager
    {
        private const string CacheFileName = "max-characters";
        private int _cachedValue = 0;

        public int GetMaxCharacters()
        {
            if (_cachedValue > 0) 
                return _cachedValue;
            if (!File.Exists(CacheFileName))
                return 0;

            var line = File.ReadAllText(CacheFileName);
            var fields = line.Split("\t");
            var isInt = int.TryParse(fields[0], out var value);
            if (!isInt) 
                return 0;

            var isDate = DateTime.TryParse(fields[1], out var date);
            if (!isDate || date.Subtract(DateTime.Now).Days >= 10) 
                return 0;

            _cachedValue = value;
            return _cachedValue;
        }

        public void SaveMaxCharacters(int value)
        {
            _cachedValue = value;
            var line = $"{value}\t{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            File.WriteAllText(CacheFileName, line);
        }
    }
}
