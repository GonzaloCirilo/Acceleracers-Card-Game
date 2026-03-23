using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class ModCardData : CardData, IAPCostCard, ISPPCard
    {
        public override CardType CardType => CardType.Mod;
        public int APCost { get; }
        public SPP SPP { get; }
        public ModabilityIcon ModabilityIcons { get; }

        public ModCardData(string id, string name, SPP spp, int apCost = 1,
            ModabilityIcon modabilityIcons = ModabilityIcon.None,
            TerrainIcon terrainIcons = TerrainIcon.None,
            IEnumerable<string> effectIds = null)
            : base(id, name, effectIds: effectIds, terrainIcons: terrainIcons)
        {
            APCost = apCost;
            SPP = spp;
            ModabilityIcons = modabilityIcons;
        }
    }
}
