using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;

namespace AcceleracersCCG.Components
{
    /// <summary>
    /// The discard/junk pile. Cards go here when destroyed, discarded, or stripped.
    /// </summary>
    public class JunkPile
    {
        private readonly List<CardInstance> _cards;

        public int Count => _cards.Count;
        public IReadOnlyList<CardInstance> Cards => _cards;

        public JunkPile()
        {
            _cards = new List<CardInstance>();
        }

        public JunkPile(IEnumerable<CardInstance> cards)
        {
            _cards = new List<CardInstance>(cards);
        }

        public void Add(CardInstance card)
        {
            _cards.Add(card);
        }

        public bool Remove(int uniqueId)
        {
            int idx = _cards.FindIndex(c => c.UniqueId == uniqueId);
            if (idx < 0) return false;
            _cards.RemoveAt(idx);
            return true;
        }

        public bool Contains(int uniqueId)
            => _cards.Any(c => c.UniqueId == uniqueId);

        public JunkPile Clone()
        {
            return new JunkPile(_cards.Select(c => c.Clone()));
        }
    }
}
