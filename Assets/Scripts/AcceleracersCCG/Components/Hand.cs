using System;
using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Components
{
    /// <summary>
    /// A player's hand of cards, with a max size for discard checks.
    /// </summary>
    public class Hand
    {
        private readonly List<CardInstance> _cards;

        public int Count => _cards.Count;
        public bool IsOverMaxSize => _cards.Count > Constants.MaxHandSize;
        public int OverCount => Math.Max(0, _cards.Count - Constants.MaxHandSize);
        public IReadOnlyList<CardInstance> Cards => _cards;

        public Hand()
        {
            _cards = new List<CardInstance>();
        }

        public Hand(IEnumerable<CardInstance> cards)
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

        public CardInstance Get(int uniqueId)
            => _cards.FirstOrDefault(c => c.UniqueId == uniqueId);

        public bool Contains(int uniqueId)
            => _cards.Any(c => c.UniqueId == uniqueId);

        public List<CardInstance> GetByType(CardType type)
            => _cards.Where(c => c.Data.CardType == type).ToList();

        public Hand Clone()
        {
            return new Hand(_cards.Select(c => c.Clone()));
        }
    }
}
