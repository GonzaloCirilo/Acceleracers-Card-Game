using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class ModCardData : CardData
    {
        public override CardType CardType => CardType.Mod;
        public ModabilityIcon ModabilityIcons { get; }

        public ModCardData(string id, string name, SPP spp, int apCost = 1,
            ModabilityIcon modabilityIcons = ModabilityIcon.None,
            TerrainIcon terrainIcons = TerrainIcon.None,
            string effectId = null)
            : base(id, name, apCost: apCost, effectId: effectId, spp: spp, terrainIcons: terrainIcons)
        {
            ModabilityIcons = modabilityIcons;
        }
    }
}
