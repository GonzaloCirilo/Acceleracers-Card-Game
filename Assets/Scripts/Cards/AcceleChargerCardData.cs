using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class AcceleChargerCardData : CardData, IAPCostCard, ISPPCard
    {
        public override CardType CardType => CardType.AcceleCharger;
        public int APCost { get; }
        public SPP SPP { get; }
        public string AcceleronSymbol { get; }

        public AcceleChargerCardData(string id, string name, SPP spp, int apCost = 1,
            TerrainIcon terrainIcons = TerrainIcon.None,
            string acceleronSymbol = null,
            IEnumerable<string> effectIds = null)
            : base(id, name, effectIds: effectIds, terrainIcons: terrainIcons)
        {
            APCost = apCost;
            SPP = spp;
            AcceleronSymbol = acceleronSymbol;
        }
    }
}
