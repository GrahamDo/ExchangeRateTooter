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
            
            var lastModifiedDate = File.GetLastWriteTime(CacheFileName);
            if (lastModifiedDate.Subtract(DateTime.Now).Days >= 10)
                return 0;

            var contents = File.ReadAllText(CacheFileName);
            var isInt = int.TryParse(contents, out var value);
            if (!isInt) 
                return 0;

            _cachedValue = value;
            return _cachedValue;
        }

        public void SaveMaxCharacters(int value)
        {
            _cachedValue = value;
            File.WriteAllText(CacheFileName, value.ToString());
        }

        public void ClearCache()
        {
            _cachedValue = 0;
            if (File.Exists(CacheFileName))
                File.Delete(CacheFileName);
        }
    }
}
