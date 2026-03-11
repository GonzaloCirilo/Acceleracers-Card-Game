using System;
using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Infrastructure;

namespace AcceleracersCCG.Components
{
    /// <summary>
    /// A shuffleable draw pile. Cards are drawn from the top (end of list).
    /// </summary>
    public class Deck
    {
        private readonly List<CardInstance> _cards;

        public int Count => _cards.Count;
        public bool IsEmpty => _cards.Count == 0;

        public Deck()
        {
            _cards = new List<CardInstance>();
        }

        public Deck(IEnumerable<CardInstance> cards)
        {
            _cards = new List<CardInstance>(cards);
        }

        public void Shuffle(IRandomProvider rng)
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public CardInstance Draw()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Cannot draw from empty deck.");
            var card = _cards[_cards.Count - 1];
            _cards.RemoveAt(_cards.Count - 1);
            return card;
        }

        public CardInstance Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Cannot peek at empty deck.");
            return _cards[_cards.Count - 1];
        }

        public void AddToTop(CardInstance card)
        {
            _cards.Add(card);
        }

        public void AddToBottom(CardInstance card)
        {
            _cards.Insert(0, card);
        }

        public bool Contains(int uniqueId)
            => _cards.Any(c => c.UniqueId == uniqueId);

        /// <summary>
        /// Remove a specific card by unique ID (for undo support).
        /// </summary>
        public bool Remove(int uniqueId)
        {
            int idx = _cards.FindIndex(c => c.UniqueId == uniqueId);
            if (idx < 0) return false;
            _cards.RemoveAt(idx);
            return true;
        }

        public List<CardInstance> ToList() => new List<CardInstance>(_cards);

        public Deck Clone()
        {
            return new Deck(_cards.Select(c => c.Clone()));
        }
    }
}
