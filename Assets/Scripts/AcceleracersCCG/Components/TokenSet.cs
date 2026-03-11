using System.Collections.Generic;
using System.Linq;

namespace AcceleracersCCG.Components
{
    /// <summary>
    /// Tracks named counters/tokens on a vehicle or card.
    /// Used for terrain markers, timed effect countdowns, etc.
    /// </summary>
    public class TokenSet
    {
        private readonly Dictionary<string, int> _tokens;

        public int Count => _tokens.Count;

        public TokenSet()
        {
            _tokens = new Dictionary<string, int>();
        }

        public TokenSet(Dictionary<string, int> tokens)
        {
            _tokens = new Dictionary<string, int>(tokens);
        }

        public int Get(string key) => _tokens.TryGetValue(key, out int val) ? val : 0;

        public void Set(string key, int value)
        {
            if (value <= 0)
                _tokens.Remove(key);
            else
                _tokens[key] = value;
        }

        public void Increment(string key, int amount = 1)
        {
            Set(key, Get(key) + amount);
        }

        public void Decrement(string key, int amount = 1)
        {
            Set(key, Get(key) - amount);
        }

        public bool Has(string key) => _tokens.ContainsKey(key) && _tokens[key] > 0;

        public void Remove(string key) => _tokens.Remove(key);

        public void Clear() => _tokens.Clear();

        public Dictionary<string, int> GetAll() => new Dictionary<string, int>(_tokens);

        /// <summary>
        /// Returns all tokens that have reached zero or below (for tick processing).
        /// </summary>
        public List<string> GetExpired()
            => _tokens.Where(kv => kv.Value <= 0).Select(kv => kv.Key).ToList();

        public TokenSet Clone() => new TokenSet(_tokens);
    }
}
