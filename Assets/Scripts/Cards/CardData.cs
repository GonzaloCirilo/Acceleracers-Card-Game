using System;
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
        public int APCost { get; }
        public string EffectId { get; }
        public SPP SPP { get; }
        public TerrainIcon TerrainIcons { get; }

        protected CardData(string id, string name, int apCost = 0, string effectId = null,
            SPP spp = default, TerrainIcon terrainIcons = TerrainIcon.None)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            APCost = apCost;
            EffectId = effectId;
            SPP = spp;
            TerrainIcons = terrainIcons;
        }
    }
}
