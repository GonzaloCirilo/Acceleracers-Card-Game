using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public class AcceleChargerCardData : CardData
    {
        public override CardType CardType => CardType.AcceleCharger;
        public string AcceleronSymbol { get; }

        public AcceleChargerCardData(string id, string name, SPP spp, int apCost = 1,
            TerrainIcon terrainIcons = TerrainIcon.None,
            string acceleronSymbol = null,
            string effectId = null)
            : base(id, name, apCost: apCost, effectId: effectId, spp: spp, terrainIcons: terrainIcons)
        {
            AcceleronSymbol = acceleronSymbol;
        }
    }
}
