namespace AcceleracersCCG.Infrastructure
{
    /// <summary>
    /// Abstraction over RNG for testability and replay.
    /// </summary>
    public interface IRandomProvider
    {
        int Next(int maxExclusive);
        int Next(int minInclusive, int maxExclusive);
    }
}
