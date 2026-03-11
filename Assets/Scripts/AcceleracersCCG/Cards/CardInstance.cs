using System;
using System.Threading;

namespace AcceleracersCCG.Cards
{
    /// <summary>
    /// Runtime wrapper: a specific copy of a card in play, with a unique identity.
    /// Multiple CardInstances can reference the same CardData.
    /// </summary>
    public class CardInstance
    {
        private static int _nextId = 0;

        public int UniqueId { get; }
        public CardData Data { get; }

        /// <summary>
        /// Resets the static ID counter. Call before starting a new game or replay.
        /// </summary>
        public static void ResetIdCounter(int startFrom = 1)
        {
            _nextId = startFrom - 1;
        }

        public CardInstance(CardData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            UniqueId = Interlocked.Increment(ref _nextId);
        }

        /// <summary>
        /// Constructor with explicit unique ID (used for deserialization/cloning).
        /// </summary>
        public CardInstance(CardData data, int uniqueId)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            UniqueId = uniqueId;
        }

        /// <summary>
        /// Creates a clone with the same unique ID (for state cloning).
        /// </summary>
        public CardInstance Clone() => new CardInstance(Data, UniqueId);

        public override string ToString() => $"[{UniqueId}] {Data.Name}";

        public override bool Equals(object obj)
            => obj is CardInstance other && UniqueId == other.UniqueId;

        public override int GetHashCode() => UniqueId;
    }
}
