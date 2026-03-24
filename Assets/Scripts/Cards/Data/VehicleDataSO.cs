using UnityEngine;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Cards.Data
{
    [CreateAssetMenu(fileName = "NewVehicle", menuName = "AcceleracersCCG/Vehicle")]
    public class VehicleDataSO : CardDataSO
    {
        [Header("SPP Stats")]
        public int speed;
        public int power;
        public int performance;

        [Header("Vehicle")]
        public Team team;
        public ModabilityIcon modabilityIcons;
        public override CardData ToCardData()
        {
            return new VehicleCardData(cardId, cardName, new SPP(speed, power, performance),
                team: team, modabilityIcons: modabilityIcons, terrainIcons: terrainIcons,
                effectIds: effectIds);
        }
    }
}
