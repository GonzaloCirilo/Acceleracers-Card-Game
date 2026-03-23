using System;
using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    /// <summary>
    /// Abstract base for all card definitions. Plain C# — no Unity dependency.
    /// Subclasses carry only the fields relevant to their card type.
    /// </summary>
    public abstract class CardData
    {
        public string Id { get; }
        public string Name { get; }
        public abstract CardType CardType { get; }
        public IReadOnlyList<string> EffectIds { get; }
        public TerrainIcon TerrainIcons { get; }

        protected CardData(string id, string name, IEnumerable<string> effectIds = null,
            TerrainIcon terrainIcons = TerrainIcon.None)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EffectIds = effectIds?.ToList() ?? new List<string>();
            TerrainIcons = terrainIcons;
        }

        /// <summary>
        /// Returns true if the card has an effect matching the given ID, with or without a parameter.
        /// Matches both "effect_id" and "effect_id:value".
        /// </summary>
        public bool HasEffect(string effectId) =>
            EffectIds.Any(e => e == effectId || e.StartsWith(effectId + ":"));

        /// <summary>
        /// Returns the integer parameter for a parameterized effect ID (e.g. "apply_countdown:4" → 4).
        /// Returns defaultValue if the effect is absent or has no parameter.
        /// </summary>
        public int GetEffectParam(string effectId, int defaultValue = 0)
        {
            var entry = EffectIds.FirstOrDefault(e => e == effectId || e.StartsWith(effectId + ":"));
            if (entry == null) return defaultValue;
            var colon = entry.IndexOf(':');
            if (colon < 0) return defaultValue;
            return int.TryParse(entry.Substring(colon + 1), out int val) ? val : defaultValue;
        }
    }
}
