using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public sealed class HazardCardData : CardData
    {
        public override CardType CardType => CardType.Hazard;
        public SPP SPPDamage { get; }
        public bool CanTargetAcceleChargers { get; }
        public bool CanTargetVehicles { get; }

        public HazardCardData(string id, string name, SPP sppDamage, int apCost = 1,
            bool canTargetAcceleChargers = false,
            bool canTargetVehicles = false,
            SPP spp = default,
            TerrainIcon terrainIcons = TerrainIcon.None,
            IEnumerable<string> effectIds = null)
            : base(id, name, apCost: apCost, effectIds: effectIds, spp: spp, terrainIcons: terrainIcons)
        {
            SPPDamage = sppDamage;
            CanTargetAcceleChargers = canTargetAcceleChargers;
            CanTargetVehicles = canTargetVehicles;
        }
    }
}
