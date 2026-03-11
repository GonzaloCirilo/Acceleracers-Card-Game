using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    /// <summary>
    /// Runtime card registry. Loaded from ScriptableObjects or populated directly for tests.
    /// </summary>
    public class CardDatabase
    {
        private readonly Dictionary<string, CardData> _cards = new Dictionary<string, CardData>();

        public int Count => _cards.Count;

        public void Register(CardData card)
        {
            _cards[card.Id] = card;
        }

        public CardData GetById(string id)
        {
            return _cards.TryGetValue(id, out var card) ? card : null;
        }

        public List<CardData> GetByType(CardType type)
        {
            return _cards.Values.Where(c => c.CardType == type).ToList();
        }

        public List<CardData> GetByTeam(Team team)
        {
            return _cards.Values
                .Where(c => c is VehicleCardData v && v.Team == team)
                .ToList();
        }

        public List<CardData> GetAll()
        {
            return new List<CardData>(_cards.Values);
        }

        /// <summary>
        /// Create a CardInstance from a registered card ID.
        /// </summary>
        public CardInstance CreateInstance(string cardId)
        {
            var data = GetById(cardId);
            if (data == null)
                throw new System.ArgumentException($"Card '{cardId}' not found in database.");
            return new CardInstance(data);
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// Load all CardDataSO assets from a Resources folder path.
        /// </summary>
        public void LoadFromResources(string path = "Cards")
        {
            var assets = UnityEngine.Resources.LoadAll<Data.CardDataSO>(path);
            foreach (var so in assets)
            {
                Register(so.ToCardData());
            }
        }
#endif
    }
}
