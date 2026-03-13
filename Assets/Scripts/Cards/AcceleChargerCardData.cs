using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class AcceleChargerCardData : CardData
    {
        public override CardType CardType => CardType.AcceleCharger;
        public string AcceleronSymbol { get; }

        public AcceleChargerCardData(string id, string name, SPP spp, int apCost = 1,
            TerrainIcon terrainIcons = TerrainIcon.None,
            string acceleronSymbol = null,
            IEnumerable<string> effectIds = null)
            : base(id, name, apCost: apCost, effectIds: effectIds, spp: spp, terrainIcons: terrainIcons)
        {
            AcceleronSymbol = acceleronSymbol;
        }
    }
}
