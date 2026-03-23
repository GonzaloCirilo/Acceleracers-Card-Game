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
        public SPP SPP { get; }
        public TerrainIcon TerrainIcons { get; }

        protected CardData(string id, string name, IEnumerable<string> effectIds = null,
            SPP spp = default, TerrainIcon terrainIcons = TerrainIcon.None)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EffectIds = effectIds?.ToList() ?? new List<string>();
            SPP = spp;
            TerrainIcons = terrainIcons;
        }

        public bool HasEffect(string effectId) => EffectIds.Contains(effectId);
    }
}
