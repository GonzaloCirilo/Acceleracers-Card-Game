using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class ShiftCardData : CardData
    {
        public override CardType CardType => CardType.Shift;

        public ShiftCardData(string id, string name, SPP spp, int apCost = 1,
            TerrainIcon terrainIcons = TerrainIcon.None,
            IEnumerable<string> effectIds = null)
            : base(id, name, apCost: apCost, effectIds: effectIds, spp: spp, terrainIcons: terrainIcons)
        {
        }
    }
}
