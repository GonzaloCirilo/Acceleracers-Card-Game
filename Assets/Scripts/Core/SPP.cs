using System;

namespace AcceleracersCCG.Core
{
    /// <summary>
    /// Speed / Power / Performance — the fundamental stat triple.
    /// Immutable value type with arithmetic operators.
    /// </summary>
    public readonly struct SPP : IEquatable<SPP>
    {
        public readonly int Speed;
        public readonly int Power;
        public readonly int Performance;

        public SPP(int speed, int power, int performance)
        {
            Speed = speed;
            Power = power;
            Performance = performance;
        }

        public static SPP Zero => new SPP(0, 0, 0);

        public static SPP operator +(SPP a, SPP b)
            => new SPP(a.Speed + b.Speed, a.Power + b.Power, a.Performance + b.Performance);

        public static SPP operator -(SPP a, SPP b)
            => new SPP(a.Speed - b.Speed, a.Power - b.Power, a.Performance - b.Performance);

        public static SPP operator -(SPP a)
            => new SPP(-a.Speed, -a.Power, -a.Performance);

        /// <summary>
        /// Returns the value for a given SPP category.
        /// </summary>
        public int GetCategory(SPPCategory category)
        {
            return category switch
            {
                SPPCategory.Speed => Speed,
                SPPCategory.Power => Power,
                SPPCategory.Performance => Performance,
                _ => throw new ArgumentOutOfRangeException(nameof(category))
            };
        }

        /// <summary>
        /// True if the specified category meets or exceeds the escape value.
        /// </summary>
        public bool MeetsEscape(int escapeValue, SPPCategory category)
            => GetCategory(category) >= escapeValue;

        /// <summary>
        /// True if any stat is zero or below (card is junked).
        /// </summary>
        public bool AnyZeroOrBelow()
            => Speed <= 0 || Power <= 0 || Performance <= 0;

        /// <summary>
        /// Returns true if the given category has a positive (non-zero) value.
        /// Used for checking SPP damage windows — blank windows are ignored.
        /// </summary>
        public bool HasWindow(SPPCategory category)
            => GetCategory(category) > 0;

        public bool Equals(SPP other)
            => Speed == other.Speed && Power == other.Power && Performance == other.Performance;

        public override bool Equals(object obj) => obj is SPP other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Speed, Power, Performance);

        public static bool operator ==(SPP a, SPP b) => a.Equals(b);
        public static bool operator !=(SPP a, SPP b) => !a.Equals(b);

        public override string ToString() => $"SPP({Speed}/{Power}/{Performance})";
    }
}
