using System;

namespace AcceleracersCCG.Infrastructure
{
    /// <summary>
    /// Deterministic RNG seeded for replay and testing.
    /// </summary>
    public class SeededRandomProvider : IRandomProvider
    {
        private readonly Random _random;

        public SeededRandomProvider(int seed)
        {
            _random = new Random(seed);
        }

        public int Next(int maxExclusive) => _random.Next(maxExclusive);

        public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    }
}
