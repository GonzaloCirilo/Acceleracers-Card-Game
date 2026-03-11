using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public class ShiftCardData : CardData
    {
        public override CardType CardType => CardType.Shift;

        public ShiftCardData(string id, string name, SPP spp, int apCost = 1,
            TerrainIcon terrainIcons = TerrainIcon.None,
            string effectId = null)
            : base(id, name, apCost: apCost, effectId: effectId, spp: spp, terrainIcons: terrainIcons)
        {
        }
    }
}
