using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class ShiftCardData : CardData, IAPCostCard
    {
        public override CardType CardType => CardType.Shift;
        public int APCost { get; }

        public ShiftCardData(string id, string name, SPP spp, int apCost = 1,
            TerrainIcon terrainIcons = TerrainIcon.None,
            IEnumerable<string> effectIds = null)
            : base(id, name, effectIds: effectIds, spp: spp, terrainIcons: terrainIcons)
        {
            APCost = apCost;
        }
    }
}
