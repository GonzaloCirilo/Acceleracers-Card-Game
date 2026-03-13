using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards.Data
{
    [CreateAssetMenu(fileName = "NewHazard", menuName = "AcceleracersCCG/Hazard")]
    public class HazardDataSO : CardDataSO
    {
        [Header("Hazard Damage")]
        public int damageSpeed;
        public int damagePower;
        public int damagePerformance;

        [Header("Hazard Targeting")]
        public bool canTargetAcceleChargers;
        public bool canTargetVehicles;

        public override CardData ToCardData()
        {
            return new HazardCardData(cardId, cardName,
                sppDamage: new SPP(damageSpeed, damagePower, damagePerformance),
                apCost: apCost,
                canTargetAcceleChargers: canTargetAcceleChargers,
                canTargetVehicles: canTargetVehicles,
                spp: new SPP(speed, power, performance),
                effectIds: effectIds);
        }
    }
}
