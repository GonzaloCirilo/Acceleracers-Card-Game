using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class HazardCardData : CardData, IAPCostCard
    {
        public override CardType CardType => CardType.Hazard;
        public int APCost { get; }
        public SPP SPPDamage { get; }
        public bool CanTargetAcceleChargers { get; }
        public bool CanTargetVehicles { get; }

        public HazardCardData(string id, string name, SPP sppDamage, int apCost = 1,
            bool canTargetAcceleChargers = false,
            bool canTargetVehicles = false,
            SPP spp = default,
            TerrainIcon terrainIcons = TerrainIcon.None,
            IEnumerable<string> effectIds = null)
            : base(id, name, effectIds: effectIds, spp: spp, terrainIcons: terrainIcons)
        {
            APCost = apCost;
            SPPDamage = sppDamage;
            CanTargetAcceleChargers = canTargetAcceleChargers;
            CanTargetVehicles = canTargetVehicles;
        }
    }
}
