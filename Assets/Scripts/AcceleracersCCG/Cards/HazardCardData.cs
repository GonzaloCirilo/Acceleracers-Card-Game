using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards
{
    public class HazardCardData : CardData
    {
        public override CardType CardType => CardType.Hazard;
        public SPP SPPDamage { get; }
        public bool CanTargetAcceleChargers { get; }
        public bool CanTargetVehicles { get; }

        public HazardCardData(string id, string name, SPP sppDamage, int apCost = 1,
            bool canTargetAcceleChargers = false,
            bool canTargetVehicles = false,
            SPP spp = default,
            string effectId = null)
            : base(id, name, apCost: apCost, effectId: effectId, spp: spp)
        {
            SPPDamage = sppDamage;
            CanTargetAcceleChargers = canTargetAcceleChargers;
            CanTargetVehicles = canTargetVehicles;
        }
    }
}
